using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.SkillDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Skills;

public class GetSkillsWithStatsQueryHandler : IRequestHandler<GetSkillsWithStatsQuery, IResult<IEnumerable<SkillWithStatsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache; 

    public GetSkillsWithStatsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IResult<IEnumerable<SkillWithStatsDto>>> Handle(GetSkillsWithStatsQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = "all_skills_stats";

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedDtos = JsonSerializer.Deserialize<IEnumerable<SkillWithStatsDto>>(cachedData);
            return Result<IEnumerable<SkillWithStatsDto>>.Success(cachedDtos);
        }

        var skills = await _unitOfWork.Skills.GetSkillsWithDetailsAsync(cancellationToken);
        var dtos = _mapper.Map<IEnumerable<SkillWithStatsDto>>(skills);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        var serializedData = JsonSerializer.Serialize(dtos);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

        return Result<IEnumerable<SkillWithStatsDto>>.Success(dtos);
    }
}