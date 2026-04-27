using AutoMapper;
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

namespace SkillBridge.Application.Queries.Categories;

public class GetCategoriesWithSkillsQueryHandler : IRequestHandler<GetCategoriesWithSkillsQuery, IResult<IEnumerable<CategoryWithSkillsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoriesWithSkillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<IEnumerable<CategoryWithSkillsDto>>> Handle(GetCategoriesWithSkillsQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Categories.GetCategoriesWithSkillsAsync(cancellationToken);

        var dtos = _mapper.Map<IEnumerable<CategoryWithSkillsDto>>(categories);

        return Result<IEnumerable<CategoryWithSkillsDto>>.Success(dtos);
    }
}