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
using SkillBridge.Domain.Enums;

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

        query = query.Where(m => m.Status == MentorStatus.Approved);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(m =>
                m.User.FirstName.ToLower().Contains(term) ||
                m.User.LastName.ToLower().Contains(term) ||
                m.CurrentJobTitle.ToLower().Contains(term) ||
                m.Company.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.SkillName))
        {
            var skill = request.SkillName.Trim().ToLower();
            query = query.Where(m => m.MentorSkills.Any(ms => ms.Skill.Name.ToLower() == skill));
        }

        if (request.MinExperience.HasValue)
        {
            query = query.Where(m => m.YearsOfExperience >= request.MinExperience.Value);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(m => m.User)
            .Include(m => m.MentorSkills).ThenInclude(ms => ms.Skill)
            .OrderByDescending(m => m.Rating) 
            .ThenByDescending(m => m.YearsOfExperience)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IEnumerable<MentorProfileListDto>>(items);

        return new PagedResult<MentorProfileListDto>(
            items: dtos,
            totalCount: totalCount,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize
        );
    }
}