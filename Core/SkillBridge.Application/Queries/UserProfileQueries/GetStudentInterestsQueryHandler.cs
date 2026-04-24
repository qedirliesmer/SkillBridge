using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Exceptions;
using SkillBridge.Application.DTOs.StudentInterestDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.UserProfileQueries;

public class GetStudentInterestsQueryHandler : IRequestHandler<GetStudentInterestsQuery, List<InterestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentInterestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<InterestDto>> Handle(GetStudentInterestsQuery request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Repository<UserProfile>()
            .GetWhere(p => p.UserId == request.UserId)
            .Include(p => p.StudentInterests)
                .ThenInclude(si => si.Skill)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
            throw new NotFoundException(nameof(UserProfile), request.UserId);

        var skills = profile.StudentInterests
            .Select(si => si.Skill)
            .ToList();

        return _mapper.Map<List<InterestDto>>(skills);
    }
}