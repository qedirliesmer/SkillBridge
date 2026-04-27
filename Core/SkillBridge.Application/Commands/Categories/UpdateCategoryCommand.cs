using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.CategoryDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Categories;

public record UpdateCategoryCommand(UpdateCategoryDto Dto) : IRequest<IResult<Unit>>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResult<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult<Unit>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Dto.Id, cancellationToken);
        if (category == null) return Result<Unit>.Failure("Category not found.");

        if (category.Name.ToLower() != request.Dto.Name.ToLower())
        {
            var nameExists = await _unitOfWork.Categories.AnyAsync(c =>
                c.Name.ToLower() == request.Dto.Name.ToLower(), cancellationToken);

            if (nameExists) return Result<Unit>.Failure("Another category with this name already exists.");
        }

        category.Name = request.Dto.Name;
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value, "Category updated successfully.");
    }
}
