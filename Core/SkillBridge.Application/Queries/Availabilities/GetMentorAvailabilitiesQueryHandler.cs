using MediatR;
using SkillBridge.Application.DTOs.AvaiabilityDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Availabilities;

public class GetMentorAvailabilitiesQueryHandler : IRequestHandler<GetMentorAvailabilitiesQuery, IEnumerable<AvailabilityDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMentorAvailabilitiesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AvailabilityDto>> Handle(GetMentorAvailabilitiesQuery request, CancellationToken cancellationToken)
    {
        var mentor = await _unitOfWork.MentorProfiles.GetByIdAsync(request.MentorId);

        if (mentor == null || mentor.Status != MentorStatus.Approved)
        {
            return Enumerable.Empty<AvailabilityDto>();
        }

        var availabilities = await _unitOfWork.Availabilities.GetByMentorIdAsync(
            request.MentorId,
            cancellationToken);

        return availabilities
            .OrderBy(a => (int)a.DayOfWeek)
            .ThenBy(a => a.StartTime)
            .Select(a => new AvailabilityDto
            {
                Id = a.Id,
                MentorId = a.MentorId,
                DayOfWeek = (int)a.DayOfWeek,
                DayOfWeekName = a.DayOfWeek.ToString(),
                StartTime = a.StartTime.ToString(@"hh\:mm"),
                EndTime = a.EndTime.ToString(@"hh\:mm")
            });
    }
}