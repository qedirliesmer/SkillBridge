using AutoMapper;
using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.SkillDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Skills;

public class GetSkillsWithStatsQueryHandler : IRequestHandler<GetSkillsWithStatsQuery, IResult<IEnumerable<SkillWithStatsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSkillsWithStatsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<IEnumerable<SkillWithStatsDto>>> Handle(GetSkillsWithStatsQuery request, CancellationToken cancellationToken)
    {
        var skills = await _unitOfWork.Skills.GetSkillsWithDetailsAsync(cancellationToken);

        var dtos = _mapper.Map<IEnumerable<SkillWithStatsDto>>(skills);

        return Result<IEnumerable<SkillWithStatsDto>>.Success(dtos);
    }
}
