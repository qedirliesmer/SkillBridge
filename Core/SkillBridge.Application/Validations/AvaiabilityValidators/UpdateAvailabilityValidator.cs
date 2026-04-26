using FluentValidation;
using SkillBridge.Application.DTOs.AvaiabilityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.AvaiabilityValidators;

public class UpdateAvailabilityValidator : AbstractValidator<UpdateAvailabilityDto>
{
    public UpdateAvailabilityValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Availability Record ID is required for update.");

        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(0, 6)
            .WithMessage("Invalid day of the week format (0-6).");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.")
            .Must(time => time.TotalHours >= 0 && time.TotalHours < 24)
            .WithMessage("Start time must be within a valid 24-hour window.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .Must(time => time.TotalHours > 0 && time.TotalHours <= 24)
            .WithMessage("End time must be within the range 00:01 to 24:00.")
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be later than the start time.")
            .Must((dto, endTime) => (endTime - dto.StartTime).TotalMinutes >= 60)
            .WithMessage("The updated slot must be at least 60 minutes long.");
    }
}