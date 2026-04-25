using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Mappings;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.BookingDTOs;
using SkillBridge.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Bookings;

public class GetMentorBookingsQueryHandler : IRequestHandler<GetMentorBookingsQuery, PaginatedList<BookingListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMentorBookingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<BookingListDto>> Handle(GetMentorBookingsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("İstifadəçi tapılmadı.");
        }

        var mentor = await _context.MentorProfiles
            .FirstOrDefaultAsync(m => m.UserId == currentUserId, cancellationToken);

        if (mentor == null)
        {
            throw new UnauthorizedAccessException("Siz mentor profilinə sahib deyilsiniz.");
        }

        var query = _context.Bookings
            .AsNoTracking()
            .Where(b => b.MentorId == mentor.Id);

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        query = query.OrderByDescending(b => b.ScheduledDate)
                     .ThenByDescending(b => b.StartTime);

        var result = await query
            .Select(b => new BookingListDto
            {
                Id = b.Id,
                PartnerFullName = b.Student.User.FirstName + " " + b.Student.User.LastName,

                PartnerJobTitleOrBio = b.Student.Bio ?? "Tələbə",

                ScheduledDate = b.ScheduledDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                MeetingLink = b.MeetingLink
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}