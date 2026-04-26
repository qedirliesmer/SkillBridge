using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.AvaiabilityDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Availabilities;

public record UpdateAvailabilityCommand(UpdateAvailabilityDto Dto) : IRequest<IResult>;

public class UpdateAvailabilityCommandHandler : IRequestHandler<UpdateAvailabilityCommand, IResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvailabilityCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var availability = await _unitOfWork.Availabilities.GetByIdAsync(request.Dto.Id, cancellationToken);

        if (availability == null)
            return Result.Failure("Availability not found.");

        if (request.Dto.StartTime >= request.Dto.EndTime)
            return Result.Failure("Start time must be earlier than end time.");

        Days dayEnum = (Days)request.Dto.DayOfWeek;

        var isOverlapping = await _unitOfWork.Availabilities.HasOverlappingSlotAsync(
            availability.MentorId,
            dayEnum, 
            request.Dto.StartTime,
            request.Dto.EndTime,
            request.Dto.Id,
            cancellationToken);

        if (isOverlapping)
            return Result.Failure("The updated time overlaps with another existing slot.");

        availability.DayOfWeek = dayEnum;
        availability.StartTime = request.Dto.StartTime;
        availability.EndTime = request.Dto.EndTime;

        _unitOfWork.Availabilities.Update(availability);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Availability updated successfully.");
    }
}