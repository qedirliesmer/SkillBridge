using AutoMapper;
using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.SkillDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Skills;

public record CreateSkillCommand(CreateSkillDto Dto) : IRequest<IResult<int>>;

public class CreateSkillCommandHandler : IRequestHandler<CreateSkillCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSkillCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<int>> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _unitOfWork.Repository<Category>().AnyAsync(c => c.Id == request.Dto.CategoryId, cancellationToken);
        if (!categoryExists) return Result<int>.Failure("Selected category not found.");

        var nameExists = await _unitOfWork.Skills.AnyAsync(s => s.Name.ToLower() == request.Dto.Name.ToLower(), cancellationToken);
        if (nameExists) return Result<int>.Failure("This skill already exists.");

        var skill = _mapper.Map<Skill>(request.Dto);
        await _unitOfWork.Skills.AddAsync(skill, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(skill.Id, "Skill created successfully.");
    }
}