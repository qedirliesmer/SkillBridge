using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.MentorProfiles;

public record UpdateMentorSkillsCommand(MentorProfileUpdateSkillsDto Dto) : IRequest<bool>;

public class UpdateMentorSkillsCommandHandler : IRequestHandler<UpdateMentorSkillsCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMentorSkillsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMentorSkillsCommand request, CancellationToken cancellationToken)
    {
        var mentorProfile = await _unitOfWork.MentorProfiles.GetMentorWithDetailsAsync(request.Dto.MentorProfileId, cancellationToken);

        if (mentorProfile == null)
            throw new Exception("Mentor profile not found.");

        if (mentorProfile.UserId != request.Dto.UserId)
            throw new Exception("Unauthorized! You can only update your own skills.");

        var skillsToRemove = mentorProfile.MentorSkills
            .Where(ms => !request.Dto.SkillIds.Contains(ms.SkillId))
            .ToList();

        foreach (var skill in skillsToRemove)
        {
            mentorProfile.MentorSkills.Remove(skill);
        }
        var currentSkillIds = mentorProfile.MentorSkills.Select(ms => ms.SkillId).ToList();
        var skillIdsToAdd = request.Dto.SkillIds.Except(currentSkillIds).ToList();

        if (skillIdsToAdd.Any())
        {

            foreach (var skillId in skillIdsToAdd)
            {
                mentorProfile.MentorSkills.Add(new MentorSkill
                {
                    MentorId = mentorProfile.Id,
                    SkillId = skillId,
                    Level = SkillLevel.Beginner
                });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}