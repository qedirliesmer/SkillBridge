using AutoMapper;
using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.MentorProfiles;

public record UpdateMentorProfileCommand(MentorProfileUpdateDto UpdateDto, string CurrentUserId, bool IsAdmin) : IRequest<bool>;

public class UpdateMentorProfileHandler : IRequestHandler<UpdateMentorProfileCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateMentorProfileHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateMentorProfileCommand request, CancellationToken cancellationToken)
    {
        var mentorProfile = await _unitOfWork.MentorProfiles.GetByIdAsync(request.UpdateDto.Id, cancellationToken);
        if (mentorProfile == null) throw new KeyNotFoundException($"ID-si {request.UpdateDto.Id} olan mentor profili tapılmadı.");

        if (mentorProfile.UserId != request.CurrentUserId && !request.IsAdmin)
        {
            throw new AuthenticationException("Siz yalnız öz profilinizdə düzəliş edə bilərsiniz.");
        }

        _mapper.Map(request.UpdateDto, mentorProfile);
        _unitOfWork.MentorProfiles.Update(mentorProfile);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result >= 0;
    }
}