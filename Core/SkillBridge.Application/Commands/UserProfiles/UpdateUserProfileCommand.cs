using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.DTOs.UserProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.UserProfiles;

public record UpdateUserProfileCommand(UpdateUserProfileDto Dto, string UserId) : IRequest<Unit>;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _unitOfWork.Repository<UserProfile>()
            .GetWhere(p => p.UserId == request.UserId)
            .Include(p => p.StudentInterests)
            .FirstOrDefaultAsync(cancellationToken);

        if (userProfile == null)
        {
            throw new KeyNotFoundException($"User profile not found.");
        }

        _mapper.Map(request.Dto, userProfile);

        if (request.Dto.InterestIds != null)
        {
            userProfile.StudentInterests.Clear();

            foreach (var interestId in request.Dto.InterestIds)
            {
                userProfile.StudentInterests.Add(new StudentInterest
                {
                    SkillId = interestId,
                    StudentId = userProfile.Id 
                });
            }
        }

        _unitOfWork.Repository<UserProfile>().Update(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}