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

public record CompleteBookingCommand(int Id) : IRequest<Unit>;

public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CompleteBookingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("Sessiya müddəti bitib. Yenidən daxil olun.");
        }

        var mentor = await _context.MentorProfiles
            .FirstOrDefaultAsync(m => m.UserId == userIdString, cancellationToken);

        if (mentor == null)
        {
            throw new UnauthorizedAccessException("Mentor profiliniz tapılmadı.");
        }

        int currentMentorId = mentor.Id;

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking ID: {request.Id} tapılmadı.");
        }

        if (booking.MentorId != currentMentorId)
        {
            throw new InvalidOperationException("Xəta: Dərsi yalnız mentor tamamlandı statusuna gətirə bilər.");
        }

        if (booking.Status == BookingStatus.Completed)
            throw new InvalidOperationException("Bu dərs artıq tamamlanıb.");

        if (booking.Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Ləğv edilmiş dərsi tamamlamaq mümkün deyil.");

        if (booking.Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Yalnız təsdiqlənmiş (Confirmed) dərslər tamamlana bilər.");

        var sessionFullEndTime = booking.ScheduledDate.Date.Add(booking.EndTime);

        if (DateTime.UtcNow < sessionFullEndTime)
        {
            throw new InvalidOperationException("Dərs hələ bitməyib. Vaxtından əvvəl tamamlamaq olmaz.");
        }

        booking.Status = BookingStatus.Completed;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}