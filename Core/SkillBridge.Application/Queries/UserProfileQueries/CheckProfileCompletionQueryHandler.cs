using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.UserProfileQueries;


public class CheckProfileCompletionQueryHandler : IRequestHandler<CheckProfileCompletionQuery, ProfileCompletionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckProfileCompletionQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileCompletionDto> Handle(CheckProfileCompletionQuery request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Repository<UserProfile>()
            .GetWhere(p => p.UserId == request.UserId)
            .Include(p => p.User)
            .Include(p => p.StudentInterests) 
            .FirstOrDefaultAsync(cancellationToken);

        var dto = new ProfileCompletionDto();

        if (profile == null)
        {
            dto.MissingSteps.Add("Profil tapılmadı.");
            return dto;
        }

        var steps = new List<(bool IsMet, string Message)>
        {
            (profile.User != null && !string.IsNullOrEmpty(profile.User.FirstName), "Adınızı daxil edin"),
            (profile.User != null && !string.IsNullOrEmpty(profile.User.LastName), "Soyadınızı daxil edin"),
            (profile.User != null && !string.IsNullOrEmpty(profile.User.PhoneNumber), "Əlaqə nömrənizi daxil edin"),

            (!string.IsNullOrEmpty(profile.ProfilePictureUrl), "Profil şəkli yükləyin"),
            (!string.IsNullOrEmpty(profile.Bio), "Haqqınızda qısa məlumat (Bio) yazın"),
            (!string.IsNullOrEmpty(profile.LinkedInUrl), "LinkedIn profil linkinizi əlavə edin"),
            (profile.StudentInterests.Any(), "Ən azı bir maraq dairəsi (Interests) seçin")
        };

        int totalSteps = steps.Count;
        int completedSteps = steps.Count(s => s.IsMet);

        dto.MissingSteps = steps.Where(s => !s.IsMet).Select(s => s.Message).ToList();

        dto.CompletionPercentage = totalSteps > 0 ? (completedSteps * 100) / totalSteps : 0;
        dto.IsComplete = completedSteps == totalSteps;

        return dto;
    }
}