using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Skills;

public record DeleteSkillCommand(int Id) : IRequest<IResult<Unit>>;

public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, IResult<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSkillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult<Unit>> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _unitOfWork.Skills.GetByIdAsync(request.Id, cancellationToken);
        if (skill == null) return Result<Unit>.Failure("Skill not found.");

        _unitOfWork.Skills.Delete(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value, "Skill deleted successfully along with its associations.");
    }
}
