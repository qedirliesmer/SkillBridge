using FluentValidation;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.MentorProfileValidators;

public class MentorProfileUpdateDtoValidator:AbstractValidator<MentorProfileUpdateDto>
{
    public MentorProfileUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Profile ID is required for updates.")
            .GreaterThan(0).WithMessage("Please enter a valid Profile ID.");

        RuleFor(x => x.YearsOfExperience)
            .InclusiveBetween(0, 50).WithMessage("Years of experience must be between 0 and 50.");

        RuleFor(x => x.CurrentJobTitle)
            .NotEmpty().WithMessage("Current job title cannot be empty.")
            .MaximumLength(100);

        RuleFor(x => x.Company)
            .NotEmpty().WithMessage("Company name is required.");
    }
}
