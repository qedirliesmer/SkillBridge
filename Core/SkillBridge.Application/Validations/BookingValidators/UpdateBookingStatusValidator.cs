using FluentValidation;
using SkillBridge.Application.DTOs.BookingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Validations.BookingValidators;

public class UpdateBookingStatusValidator : AbstractValidator<UpdateBookingStatusDto>
{
    private readonly string[] _allowedStatuses = { "Pending", "Confirmed", "Cancelled", "Completed" };

    public UpdateBookingStatusValidator()
    {
        RuleFor(x => x.BookingId).GreaterThan(0);

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => _allowedStatuses.Contains(status))
            .WithMessage($"Allowed statuses: {string.Join(", ", _allowedStatuses)}");

        RuleFor(x => x.MeetingLink)
            .NotEmpty()
            .When(x => x.Status == "Confirmed")
            .WithMessage("Meeting link is required when status is Confirmed.")
            .Matches(@"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$")
            .WithMessage("Invalid URL format.")
            .When(x => !string.IsNullOrEmpty(x.MeetingLink));

        RuleFor(x => x.MeetingLink)
            .Empty()
            .When(x => x.Status == "Cancelled" || x.Status == "Pending")
            .WithMessage("Meeting link should be empty for this status.");
    }
}
