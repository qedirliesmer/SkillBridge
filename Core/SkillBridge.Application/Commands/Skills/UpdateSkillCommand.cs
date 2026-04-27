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

public record UpdateSkillCommand(UpdateSkillDto Dto) : IRequest<IResult<Unit>>;

public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, IResult<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSkillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult<Unit>> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _unitOfWork.Skills.GetByIdAsync(request.Dto.Id, cancellationToken);
        if (skill == null) return Result<Unit>.Failure("Skill not found.");

        if (skill.Name.ToLower() != request.Dto.Name.ToLower())
        {
            var nameExists = await _unitOfWork.Skills.AnyAsync(s => s.Name.ToLower() == request.Dto.Name.ToLower(), cancellationToken);
            if (nameExists) return Result<Unit>.Failure("Another skill with this name already exists.");
        }
        var categoryExists = await _unitOfWork.Repository<Category>().AnyAsync(c => c.Id == request.Dto.CategoryId, cancellationToken);
        if (!categoryExists) return Result<Unit>.Failure("Selected category not found.");

        skill.Name = request.Dto.Name;
        skill.CategoryId = request.Dto.CategoryId;

        _unitOfWork.Skills.Update(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value, "Skill updated successfully.");
    }
}