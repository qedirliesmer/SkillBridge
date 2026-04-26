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

public class GetAvailabilityByIdQueryHandler : IRequestHandler<GetAvailabilityByIdQuery, AvailabilityDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAvailabilityByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AvailabilityDto> Handle(GetAvailabilityByIdQuery request, CancellationToken cancellationToken)
    {
        var availability = await _unitOfWork.Availabilities.GetByIdAsync(request.Id);
        if (availability == null) return null;

        var mentor = await _unitOfWork.MentorProfiles.GetByIdAsync(availability.MentorId);
        if (mentor == null || mentor.Status != MentorStatus.Approved)
        {
            return null;
        }

        return new AvailabilityDto
        {
            Id = availability.Id,
            MentorId = availability.MentorId,
            DayOfWeek = (int)availability.DayOfWeek, 
            DayOfWeekName = availability.DayOfWeek.ToString(),
            StartTime = availability.StartTime.ToString(@"hh\:mm"),
            EndTime = availability.EndTime.ToString(@"hh\:mm")
        };
    }
}