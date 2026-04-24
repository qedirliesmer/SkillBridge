using FluentValidation;
using SkillBridge.Application.DTOs.UserProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Application.Common.Helpers;
using SkillBridge.Application.Validations.Extensions;
namespace SkillBridge.Application.Validations.UserProfileValidators;

public class CreateUserProfileValidator : AbstractValidator<CreateUserProfileDto>
{
    public CreateUserProfileValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.");

        RuleFor(x => x.ProfilePictureUrl)
            .ValidLink().WithMessage("Please enter a valid image URL (http/https).")
            .When(x => !string.IsNullOrEmpty(x.ProfilePictureUrl));

        RuleFor(x => x.LinkedInUrl)
            .LinkedInUrl()
            .When(x => !string.IsNullOrEmpty(x.LinkedInUrl));

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("Time zone is required.");

        RuleFor(x => x.InterestIds)
            .NotNull().WithMessage("Interests selection is required.")
            .Must(x => x != null && x.Count > 0).WithMessage("Please select at least one interest.")
            .Must(x => x.Count <= 10).WithMessage("You can select a maximum of 10 interests.");
    }
}