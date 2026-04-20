using AutoMapper;
using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.MentorProfiles;

public record CreateMentorProfileCommand(MentorProfileCreateDto CreateDto, string UserId) : IRequest<int>;

public class CreateMentorProfileCommandHandler : IRequestHandler<CreateMentorProfileCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateMentorProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateMentorProfileCommand request, CancellationToken cancellationToken)
    {
        var isAlreadyMentor = await _unitOfWork.MentorProfiles
            .AnyAsync(m => m.UserId == request.UserId, cancellationToken);

        if (isAlreadyMentor)
        {
            throw new InvalidOperationException("User already has an active mentor profile.");
        }

        var mentorEntity = _mapper.Map<MentorProfile>(request.CreateDto);

        mentorEntity.UserId = request.UserId;
        mentorEntity.Rating = 0.00m;
        
        mentorEntity.Status = MentorStatus.Pending;

        if (request.CreateDto.SkillIds != null && request.CreateDto.SkillIds.Any())
        {
            mentorEntity.MentorSkills = request.CreateDto.SkillIds.Select(skillId => new MentorSkill
            {
                SkillId = skillId,
                Level = SkillLevel.Beginner
            }).ToList();
        }

        await _unitOfWork.MentorProfiles.AddAsync(mentorEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mentorEntity.Id;
    }
}