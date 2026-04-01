using AutoMapper;
using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.MentorProfiles;

public class GetMentorProfileByIdQueryHandler : IRequestHandler<GetMentorProfileByIdQuery, MentorProfileDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMentorProfileByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MentorProfileDetailDto> Handle(GetMentorProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var mentor = await _unitOfWork.MentorProfiles.GetMentorWithDetailsAsync(request.Id, cancellationToken);

        if (mentor == null)
        {
            throw new KeyNotFoundException($"Mentor profile with ID {request.Id} was not found.");
        }
        return _mapper.Map<MentorProfileDetailDto>(mentor);
    }
}