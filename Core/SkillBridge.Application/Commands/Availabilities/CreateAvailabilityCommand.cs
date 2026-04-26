using AutoMapper;
using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.AvaiabilityDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Availabilities;

public record CreateAvailabilityCommand(CreateAvailabilityDto Dto) : IRequest<IResult<int>>;

public class CreateAvailabilityCommandHandler : IRequestHandler<CreateAvailabilityCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateAvailabilityCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<int>> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.StartTime >= request.Dto.EndTime)
        {
            return Result<int>.Failure("Start time must be earlier than end time.");
        }

        var isOverlapping = await _unitOfWork.Availabilities.HasOverlappingSlotAsync(
            request.Dto.MentorId,
            (Days)request.Dto.DayOfWeek,
            request.Dto.StartTime,
            request.Dto.EndTime,
            null,
            cancellationToken);

        if (isOverlapping)
        {
            return Result<int>.Failure("This time slot overlaps with an existing availability for this mentor.");
        }

        var availability = _mapper.Map<Availability>(request.Dto);

        await _unitOfWork.Availabilities.AddAsync(availability, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(availability.Id, "Availability created successfully.");
    }
}