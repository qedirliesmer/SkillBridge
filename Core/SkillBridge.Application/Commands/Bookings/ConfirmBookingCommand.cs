using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Interfaces;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Bookings;

public record ConfirmBookingCommand(int Id) : IRequest<Unit>;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ConfirmBookingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var identityUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(identityUserId))
            throw new UnauthorizedAccessException("Bu əməliyyatı yerinə yetirmək üçün sistemə daxil olmalısınız.");

        var mentor = await _context.MentorProfiles
            .FirstOrDefaultAsync(m => m.UserId == identityUserId, cancellationToken);

        if (mentor == null)
            throw new UnauthorizedAccessException("Sizin mentor profiliniz tapılmadı.");

        int currentMentorId = mentor.Id; 

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
            throw new KeyNotFoundException($"Sistemdə {request.Id} nömrəli müraciət tapılmadı.");

        if (booking.MentorId != currentMentorId)
            throw new InvalidOperationException("Siz yalnız sizə ünvanlanmış müraciətləri təsdiqləyə bilərsiniz.");

        if (booking.Status != BookingStatus.Pending)
            throw new InvalidOperationException($"Bu müraciət artıq {booking.Status} statusundadır və təsdiqlənə bilməz.");

        bool hasConflict = await _context.Bookings.AnyAsync(b =>
            b.Id != booking.Id &&
            b.MentorId == currentMentorId &&
            b.Status == BookingStatus.Confirmed &&
            b.ScheduledDate.Date == booking.ScheduledDate.Date &&
            ((booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
             (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
             (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)),
            cancellationToken);

        if (hasConflict)
            throw new InvalidOperationException("Təsdiqləmə uğursuz oldu: Seçilmiş saat aralığında artıq təsdiqlənmiş başqa bir görüşünüz mövcuddur.");

        booking.Status = BookingStatus.Confirmed;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}