using AutoMapper;
using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.MentorProfiles;

public record CreateMentorProfileCommand(MentorProfileCreateDto CreateDto): IRequest<int>;
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
            .AnyAsync(m => m.UserId == request.CreateDto.UserId, cancellationToken);

        if (isAlreadyMentor)
        {
            throw new Exception("A mentor profile already exists for this user.");
        }

        var mentorEntity = _mapper.Map<MentorProfile>(request.CreateDto);
        mentorEntity.Rating = 0.00m;

        await _unitOfWork.MentorProfiles.AddAsync(mentorEntity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mentorEntity.Id;
    }
}
