using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Reviews;

public record DeleteReviewCommand(int Id) : IRequest<IResult<Unit>>;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, IResult<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IResult<Unit>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Repository<Review>().GetByIdAsync(request.Id, cancellationToken);
        if (review == null) return Result<Unit>.Failure("Review not found.");

        var mentorId = review.ToMentorProfileId;
        _unitOfWork.Repository<Review>().Delete(review);

        var mentor = await _unitOfWork.MentorProfiles.GetByIdAsync(mentorId, cancellationToken);
        if (mentor != null)
        {
            var otherReviews = _unitOfWork.Repository<Review>()
                .GetWhere(r => r.ToMentorProfileId == mentorId && r.Id != request.Id);

            int count = await otherReviews.CountAsync(cancellationToken);
            if (count > 0)
            {
                int sum = await otherReviews.SumAsync(r => r.Rating, cancellationToken);
                mentor.Rating = Math.Round((decimal)sum / count, 2);
            }
            else mentor.Rating = 0;

            _unitOfWork.MentorProfiles.Update(mentor);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value, "Review deleted.");
    }
}