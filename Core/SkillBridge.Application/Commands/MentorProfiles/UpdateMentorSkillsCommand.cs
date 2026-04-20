using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.MentorProfiles;
public record UpdateMentorSkillsCommand(MentorProfileUpdateSkillsDto Dto, string CurrentUserId, bool IsAdmin) : IRequest<bool>;

public class UpdateMentorSkillsCommandHandler : IRequestHandler<UpdateMentorSkillsCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateMentorSkillsCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateMentorSkillsCommand request, CancellationToken cancellationToken)
    {
        var mentorProfile = await _unitOfWork.MentorProfiles.GetMentorWithDetailsAsync(request.Dto.MentorProfileId, cancellationToken);

        if (mentorProfile == null) throw new KeyNotFoundException("Mentor profili tapılmadı.");

        if (mentorProfile.UserId != request.CurrentUserId && !request.IsAdmin)
        {
            throw new AuthenticationException("Siz bu profilin bacarıqlarını dəyişmək səlahiyyətinə malik deyilsiniz.");
        }

        const int MaxSkillsAllowed = 15;
        if (request.Dto.SkillIds.Count > MaxSkillsAllowed)
            throw new InvalidOperationException($"Maksimum {MaxSkillsAllowed} bacarıq əlavə edə bilərsiniz.");

        var skillsToRemove = mentorProfile.MentorSkills.Where(ms => !request.Dto.SkillIds.Contains(ms.SkillId)).ToList();
        foreach (var skill in skillsToRemove) mentorProfile.MentorSkills.Remove(skill);

        var currentSkillIds = mentorProfile.MentorSkills.Select(ms => ms.SkillId).ToHashSet();
        var skillIdsToAdd = request.Dto.SkillIds.Where(id => !currentSkillIds.Contains(id)).ToList();

        foreach (var skillId in skillIdsToAdd)
        {
            mentorProfile.MentorSkills.Add(new MentorSkill
            {
                MentorId = mentorProfile.Id,
                SkillId = skillId,
                Level = SkillLevel.Beginner,
                CreatedAt = DateTime.UtcNow
            });
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result >= 0;
    }
}