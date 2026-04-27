using FluentValidation;
using SkillBridge.Application.DTOs.ReviewDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.ReviewValidators;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.")
            .GreaterThan(0).WithMessage("Invalid Booking ID.");

        RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("Rating is required.")
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .When(x => x.Rating <= 2)
            .WithMessage("Please provide a comment for low ratings to help our mentors improve.");
    }
}
