using FluentValidation;
using SkillBridge.Application.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.CategoryValidators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MinimumLength(2).WithMessage("Category name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s&/-]+$").WithMessage("Category name contains invalid characters.");
    }
}
