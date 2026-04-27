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

public class GetReviewDetailQueryHandler : IRequestHandler<GetReviewDetailQuery, IResult<ReviewDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReviewDetailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<ReviewDetailDto>> Handle(GetReviewDetailQuery request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Reviews.GetReviewWithDetailsAsync(request.Id, cancellationToken);

        if (review == null)
            return Result<ReviewDetailDto>.Failure("Review not found.");

        var dto = _mapper.Map<ReviewDetailDto>(review);

        return Result<ReviewDetailDto>.Success(dto);
    }
}
