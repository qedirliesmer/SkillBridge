using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Exceptions;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.DTOs.BookingDTOs;
using SkillBridge.Application.Interfaces;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Bookings;

public class GetBookingDetailQueryHandler : IRequestHandler<GetBookingDetailQuery, BookingDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetBookingDetailQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<BookingDetailDto> Handle(GetBookingDetailQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("Sessiya aktiv deyil.");
        }

        var bookingDetail = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.Id == request.Id &&
                       (b.Mentor.UserId == currentUserId || b.Student.UserId == currentUserId))
            .Select(b => new BookingDetailDto
            {
                Id = b.Id,
                MentorId = b.MentorId,
                MentorFullName = b.Mentor.User.FirstName + " " + b.Mentor.User.LastName,
                MentorJobTitle = b.Mentor.CurrentJobTitle,
                MentorCompany = b.Mentor.Company,

                StudentId = b.StudentId,
                StudentFullName = b.Student.User.FirstName + " " + b.Student.User.LastName,
                StudentProfilePicture = b.Student.ProfilePictureUrl,

                ScheduledDate = b.ScheduledDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                MeetingLink = b.MeetingLink,

                IsReviewed = b.Review != null,
                ReviewRating = b.Review != null ? (double?)b.Review.Rating : null,
                ReviewComment = b.Review != null ? b.Review.Comment : null,
                IsCurrentUserMentor = b.Mentor.UserId == currentUserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (bookingDetail == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }

        return bookingDetail;
    }
}