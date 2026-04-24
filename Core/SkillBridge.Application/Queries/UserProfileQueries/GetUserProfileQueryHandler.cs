using AutoMapper;
using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.DTOs.UserProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Application.Interfaces;
using SkillBridge.Application.Common.Exceptions;

namespace SkillBridge.Application.Queries.UserProfileQueries;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, MyProfileDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MyProfileDetailDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Repository<UserProfile>()
            .GetAllQueryable()
            .Include(p => p.User)
            .Include(p => p.MentorProfile) 
                .ThenInclude(m => m.MentorSkills).ThenInclude(ms => ms.Skill)
            .Include(p => p.MentorProfile)
                .ThenInclude(m => m.ReviewsReceived)
            .Include(p => p.StudentInterests).ThenInclude(si => si.Skill)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
            throw new NotFoundException(nameof(UserProfile), request.UserId);

        return _mapper.Map<MyProfileDetailDto>(profile);
    }
}