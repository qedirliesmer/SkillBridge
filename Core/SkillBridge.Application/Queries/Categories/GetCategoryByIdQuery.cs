using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Categories;

public record GetCategoryByIdQuery(int Id) : IRequest<IResult<CategoryWithSkillsDto>>;
