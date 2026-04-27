using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.DTOs.SkillDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Skills;

public record GetSkillByIdQuery(int Id) : IRequest<IResult<SkillDto>>;