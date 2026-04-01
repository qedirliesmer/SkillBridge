using AutoMapper;
using MediatR;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SkillBridge.Application.Queries.MentorProfiles;

public class GetMentorsPagedQueryHandler : IRequestHandler<GetMentorsPagedQuery, PagedResult<MentorProfileListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMentorsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<MentorProfileListDto>> Handle(GetMentorsPagedQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.MentorProfiles.GetAllQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm?.ToLower();
            query = query.Where(m => m.CurrentJobTitle.ToLower().Contains(term) ||
                                     m.Company.ToLower().Contains(term));
        }
        if (!string.IsNullOrWhiteSpace(request.SkillName))
        {
            var skill = request.SkillName.ToLower();
            query = query.Where(m => m.MentorSkills.Any(ms => ms.Skill.Name.ToLower() == skill));
        }

        if (request.MinExperience.HasValue)
        {
            query = query.Where(m => m.YearsOfExperience >= request.MinExperience.Value);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
      .OrderByDescending(m => m.Rating)  
      .ThenByDescending(m => m.YearsOfExperience) 
      .ThenBy(m => m.Id) 
      .Skip((request.PageNumber - 1) * request.PageSize)
      .Take(request.PageSize)
      .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IEnumerable<MentorProfileListDto>>(items);

        return new PagedResult<MentorProfileListDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}