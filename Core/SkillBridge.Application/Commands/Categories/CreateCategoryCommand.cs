using AutoMapper;
using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.CategoryDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Categories;

public record CreateCategoryCommand(CreateCategoryDto Dto) : IRequest<IResult<int>>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await _unitOfWork.Categories.AnyAsync(c =>
            c.Name.ToLower() == request.Dto.Name.ToLower(), cancellationToken);

        if (nameExists)
            return Result<int>.Failure("A category with this name already exists.");

        var category = _mapper.Map<Category>(request.Dto);

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(category.Id, "Category created successfully.");
    }
}