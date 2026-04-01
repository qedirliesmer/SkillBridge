using AutoMapper;
using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.MentorProfiles;

public record UpdateMentorProfileCommand(MentorProfileUpdateDto UpdateDto) : IRequest<bool>;
public class UpdateMentorProfileCommandHandler : IRequestHandler<UpdateMentorProfileCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateMentorProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateMentorProfileCommand request, CancellationToken cancellationToken)
    {
        var mentorProfile = await _unitOfWork.MentorProfiles.GetByIdAsync(request.UpdateDto.Id, cancellationToken);

        if (mentorProfile == null)
        {
            throw new Exception("Mentor profile not found.");
        }

        if (mentorProfile.UserId != request.UpdateDto.UserId)
        {
            throw new Exception("You are not authorized to update this profile.");
        }

        _mapper.Map(request.UpdateDto, mentorProfile);

        _unitOfWork.MentorProfiles.Update(mentorProfile);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
