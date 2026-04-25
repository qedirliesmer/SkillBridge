using FluentValidation;
using SkillBridge.Application.DTOs.BookingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.BookingValidators;

public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.MentorId)
            .GreaterThan(0).WithMessage("Please select a valid mentor.");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("Scheduled date is required.")
            .Must(date => date.Date >= DateTime.Now.Date)
            .WithMessage("Scheduled date cannot be in the past.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.")
            .Must((dto, startTime) =>
            {
                if (dto.ScheduledDate.Date == DateTime.Now.Date)
                {
                    return startTime > DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(30));
                }
                return true;
            })
            .WithMessage("For today's bookings, start time must be at least 30 minutes from now.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be later than start time.");

        RuleFor(x => x)
            .Must(x => {
                var duration = (x.EndTime - x.StartTime).TotalMinutes;
                return duration >= 15 && duration <= 240;
            })
            .WithMessage("The session duration must be between 15 minutes and 4 hours.");
    }
}
