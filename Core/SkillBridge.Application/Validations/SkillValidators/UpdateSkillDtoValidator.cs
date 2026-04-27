using FluentValidation;
using SkillBridge.Application.DTOs.SkillDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.SkillValidators;

public class UpdateSkillDtoValidator : AbstractValidator<UpdateSkillDto>
{
    public UpdateSkillDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid skill identifier.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required.")
            .MinimumLength(2).WithMessage("Skill name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("Skill name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z0-9+#.\s-]+$").WithMessage("Skill name contains invalid characters."); 

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category must be selected.");
    }
}
