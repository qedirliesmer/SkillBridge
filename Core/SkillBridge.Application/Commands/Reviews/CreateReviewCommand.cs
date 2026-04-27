using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.ReviewDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Reviews;

public record CreateReviewCommand(CreateReviewDto Dto) : IRequest<IResult<int>>;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<int>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetWhere(b => b.Id == request.Dto.BookingId)
            .Include(b => b.Mentor) 
            .FirstOrDefaultAsync(cancellationToken);

        if (booking == null) return Result<int>.Failure("Booking not found.");

        if (booking.Status != BookingStatus.Completed)
            return Result<int>.Failure("You can only review completed sessions.");

        var alreadyReviewed = await _unitOfWork.Repository<Review>()
            .AnyAsync(r => r.BookingId == request.Dto.BookingId, cancellationToken);

        if (alreadyReviewed) return Result<int>.Failure("This session has already been reviewed.");

        var review = _mapper.Map<Review>(request.Dto);
        review.FromUserProfileId = booking.StudentId;
        review.ToMentorProfileId = booking.MentorId;

        await _unitOfWork.Repository<Review>().AddAsync(review, cancellationToken);

        var mentor = booking.Mentor; 
        if (mentor != null)
        {
            var reviewStats = await _unitOfWork.Repository<Review>()
                .GetWhere(r => r.ToMentorProfileId == mentor.Id)
                .GroupBy(r => r.ToMentorProfileId)
                .Select(g => new {
                    Count = g.Count(),
                    Sum = g.Sum(r => r.Rating)
                })
                .FirstOrDefaultAsync(cancellationToken);

            int totalCount = (reviewStats?.Count ?? 0) + 1;
            int totalSum = (reviewStats?.Sum ?? 0) + request.Dto.Rating;

            mentor.Rating = Math.Round((decimal)totalSum / totalCount, 2);
            _unitOfWork.MentorProfiles.Update(mentor);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(review.Id, "Review submitted successfully.");
    }
}