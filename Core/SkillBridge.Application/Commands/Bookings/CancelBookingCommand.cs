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

public record CancelBookingCommand(int Id, string Reason) : IRequest<Unit>;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CancelBookingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("Sessiya müddəti bitib. Yenidən daxil olun.");
        }

        var booking = await _context.Bookings
            .Include(b => b.Mentor)
            .Include(b => b.Student)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking ID {request.Id} tapılmadı.");
        }

        var studentProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(s => s.UserId == userIdString, cancellationToken);

        var mentorProfile = await _context.MentorProfiles
            .FirstOrDefaultAsync(m => m.UserId == userIdString, cancellationToken);

        bool isOwnerStudent = studentProfile != null && booking.StudentId == studentProfile.Id;
        bool isOwnerMentor = mentorProfile != null && booking.MentorId == mentorProfile.Id;

        if (!isOwnerStudent && !isOwnerMentor)
        {
            throw new InvalidOperationException("Bu dərsi ləğv etmək icazəniz yoxdur.");
        }

        if (booking.Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Bu dərs artıq ləğv edilib.");

        if (booking.Status == BookingStatus.Completed)
            throw new InvalidOperationException("Tamamlanmış dərsi ləğv etmək olmaz.");

        var sessionStartTime = booking.ScheduledDate.Date.Add(booking.StartTime);
        var utcNow = DateTime.UtcNow;

        if (sessionStartTime < utcNow)
            throw new InvalidOperationException("Keçmiş dərsi ləğv etmək olmaz.");

        if ((sessionStartTime - utcNow).TotalMinutes < 30)
            throw new InvalidOperationException("Dərsin başlamasına 30 dəqiqədən az vaxt qaldığı üçün ləğv etmək mümkün deyil.");

        booking.Status = BookingStatus.Cancelled;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}