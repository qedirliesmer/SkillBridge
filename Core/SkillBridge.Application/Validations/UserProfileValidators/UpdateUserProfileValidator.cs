using FluentValidation;
using SkillBridge.Application.Common.Helpers;
using SkillBridge.Application.DTOs.UserProfileDTOs;
using SkillBridge.Application.Validations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.UserProfileValidators;

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileValidator()
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
            .NotEmpty().WithMessage("Time zone cannot be empty.");

        RuleFor(x => x.InterestIds)
            .NotEmpty().WithMessage("Interests list cannot be empty.");
    }
}