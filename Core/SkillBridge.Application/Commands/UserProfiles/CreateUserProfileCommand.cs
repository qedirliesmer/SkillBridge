using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.DTOs.UserProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.UserProfiles;

public record CreateUserProfileCommand(CreateUserProfileDto Dto, string UserId) : IRequest<int>;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public CreateUserProfileCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<int> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        var existingProfile = await _unitOfWork.Repository<UserProfile>()
            .GetWhere(p => p.UserId == request.UserId)
            .AnyAsync(cancellationToken);

        if (existingProfile)
        {
            throw new InvalidOperationException("User profile already exists.");
        }

        var userProfile = _mapper.Map<UserProfile>(request.Dto);
        userProfile.UserId = request.UserId;

        if (request.Dto.InterestIds != null && request.Dto.InterestIds.Any())
        {
            userProfile.StudentInterests = request.Dto.InterestIds.Select(id => new StudentInterest
            {
                SkillId = id,
            }).ToList();
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("Student"))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, "Student");
            if (!roleResult.Succeeded)
            {
                throw new Exception("Failed to assign Student role.");
            }
        }
        await _unitOfWork.Repository<UserProfile>().AddAsync(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return userProfile.Id;
    }
}