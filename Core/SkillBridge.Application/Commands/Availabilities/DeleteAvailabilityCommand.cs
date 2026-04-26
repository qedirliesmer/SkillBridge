using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Availabilities;

public record DeleteAvailabilityCommand(int Id) : IRequest<IResult>;

public class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand, IResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAvailabilityCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var availability = await _unitOfWork.Availabilities.GetByIdAsync(request.Id, cancellationToken);

        if (availability == null)
            return Result.Failure("Availability schedule not found.");

        var allBookings = await _unitOfWork.Bookings.GetAllAsync(cancellationToken);

        var hasActiveBooking = allBookings.Any(b =>
            b.MentorId == availability.MentorId &&
            b.ScheduledDate.Date >= DateTime.Now.Date &&
            ((int)b.ScheduledDate.DayOfWeek == (int)availability.DayOfWeek ||
             (b.ScheduledDate.DayOfWeek == DayOfWeek.Sunday && (int)availability.DayOfWeek == 7)) &&
            b.StartTime == availability.StartTime &&
            (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed));

        if (hasActiveBooking)
        {
            return Result.Failure("Cannot delete this schedule because there is an active booking associated with it.");
        }

        _unitOfWork.Availabilities.Delete(availability);

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult <= 0)
            return Result.Failure("An error occurred while deleting the availability schedule.");

        return Result.Success("Availability schedule deleted successfully.");
    }
}