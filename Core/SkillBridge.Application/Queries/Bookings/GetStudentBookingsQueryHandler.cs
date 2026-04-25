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

public class GetStudentBookingsQueryHandler : IRequestHandler<GetStudentBookingsQuery, PaginatedList<BookingListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetStudentBookingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<BookingListDto>> Handle(GetStudentBookingsQuery request, CancellationToken cancellationToken)
    {
        var identityUserId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(identityUserId))
        {
            throw new UnauthorizedAccessException("İstifadəçi tapılmadı.");
        }

        var studentProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(s => s.UserId == identityUserId, cancellationToken);

        if (studentProfile == null)
        {
            throw new UnauthorizedAccessException("Bu istifadəçiyə aid tələbə profili tapılmadı.");
        }

        var query = _context.Bookings
            .AsNoTracking()
            .Where(b => b.StudentId == studentProfile.Id);

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
                PartnerFullName = b.Mentor.User.FirstName + " " + b.Mentor.User.LastName,
                PartnerJobTitleOrBio = b.Mentor.CurrentJobTitle ?? "Mentor",
                ScheduledDate = b.ScheduledDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString()
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}