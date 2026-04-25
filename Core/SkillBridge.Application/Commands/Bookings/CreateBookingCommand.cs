using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Exceptions;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Interfaces;
using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Bookings;

public record CreateBookingCommand(
    int MentorId,
    DateTime ScheduledDate,
    TimeSpan StartTime,
    TimeSpan EndTime) : IRequest<int>;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateBookingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString))
            throw new UnauthorizedAccessException("User identification failed. Please log in.");

        var studentProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(s => s.UserId == userIdString, cancellationToken);

        if (studentProfile == null)
            throw new UnauthorizedAccessException("Sizin tələbə profiliniz tapılmadı.");

        int studentId = studentProfile.Id;

        var requestedDateTime = request.ScheduledDate.Date.Add(request.StartTime);
        if (requestedDateTime <= DateTime.UtcNow)
            throw new InvalidOperationException("You cannot schedule a booking for a past date or time.");

        if (request.EndTime <= request.StartTime)
            throw new InvalidOperationException("Session end time must be strictly after the start time.");

        var mentor = await _context.MentorProfiles
            .FirstOrDefaultAsync(m => m.Id == request.MentorId, cancellationToken);

        if (mentor == null)
            throw new KeyNotFoundException("The requested mentor profile does not exist.");

        // Öz-özünə rezervasiya yoxlaması
        if (request.MentorId == studentId)
            throw new InvalidOperationException("Self-mentorship is not permitted. You cannot book your own services.");

        bool hasConflict = await _context.Bookings.AnyAsync(b =>
            b.MentorId == request.MentorId &&
            b.ScheduledDate.Date == request.ScheduledDate.Date &&
            b.Status != BookingStatus.Cancelled &&
            b.Status != BookingStatus.Rejected &&
            ((request.StartTime >= b.StartTime && request.StartTime < b.EndTime) ||
             (request.EndTime > b.StartTime && request.EndTime <= b.EndTime) ||
             (request.StartTime <= b.StartTime && request.EndTime >= b.EndTime)),
            cancellationToken);

        if (hasConflict)
            throw new InvalidOperationException("The requested time slot is unavailable. The mentor has a conflicting schedule.");

        var booking = new Booking
        {
            MentorId = request.MentorId,
            StudentId = studentId,
            ScheduledDate = request.ScheduledDate.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}