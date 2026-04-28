using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

public class GetSkillByIdQueryHandler : IRequestHandler<GetSkillByIdQuery, IResult<SkillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache; 

    public GetSkillByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IResult<SkillDto>> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"skill_{request.Id}";

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedDto = JsonSerializer.Deserialize<SkillDto>(cachedData);
            return Result<SkillDto>.Success(cachedDto);
        }

        var skill = await _unitOfWork.Skills
            .GetWhere(s => s.Id == request.Id)
            .Include(s => s.Category)
            .Include(s => s.MediaItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (skill == null)
            return Result<SkillDto>.Failure("Skill not found.");

        var dto = _mapper.Map<SkillDto>(skill);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };

        var serializedData = JsonSerializer.Serialize(dto);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

        return Result<SkillDto>.Success(dto);
    }
}
