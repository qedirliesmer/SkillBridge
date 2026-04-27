using AutoMapper;
using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.ReviewDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Reviews;

public class GetMentorReviewsQueryHandler : IRequestHandler<GetMentorReviewsQuery, IResult<IEnumerable<MentorProfileReviewDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMentorReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<IEnumerable<MentorProfileReviewDto>>> Handle(GetMentorReviewsQuery request, CancellationToken cancellationToken)
    {
        // Repository qatında yazdığımız xüsusi metodu istifadə edirik
        var reviews = await _unitOfWork.Reviews.GetReviewsByMentorIdAsync(request.MentorId, cancellationToken);

        var dtos = _mapper.Map<IEnumerable<MentorProfileReviewDto>>(reviews);

        return Result<IEnumerable<MentorProfileReviewDto>>.Success(dtos);
    }
}
