using AutoMapper;
using MediatR;
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

public class GetTopRatedMentorsQueryHandler : IRequestHandler<GetTopRatedMentorsQuery, IEnumerable<MentorProfileListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTopRatedMentorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MentorProfileListDto>> Handle(GetTopRatedMentorsQuery request, CancellationToken cancellationToken)
    {
        var topMentors = await _unitOfWork.MentorProfiles.GetAllQueryable()
            .AsNoTracking()
            .Where(m => m.Status == MentorStatus.Approved)
            .Include(m => m.User)
            .Include(m => m.MentorSkills)
                .ThenInclude(ms => ms.Skill)
            .OrderByDescending(m => m.Rating)
            .ThenByDescending(m => m.YearsOfExperience)
            .Take(request.Count)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<MentorProfileListDto>>(topMentors);
    }
}