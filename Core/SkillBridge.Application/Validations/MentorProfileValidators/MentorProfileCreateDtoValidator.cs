using FluentValidation;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.MentorProfileValidators;

public class MentorProfileCreateDtoValidator:AbstractValidator<MentorProfileCreateDto>
{
   
    public MentorProfileCreateDtoValidator()
    {

        RuleFor(x => x.YearsOfExperience)
            .InclusiveBetween(0, 50).WithMessage("Years of experience must be between 0 and 50.");

        RuleFor(x => x.CurrentJobTitle)
            .NotEmpty().WithMessage("Current job title cannot be empty.")
            .MinimumLength(2).WithMessage("Job title is too short.")
            .MaximumLength(100).WithMessage("Job title must not exceed 100 characters.");

        RuleFor(x => x.Company)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(150).WithMessage("Company name must not exceed 150 characters.");
    }
}
