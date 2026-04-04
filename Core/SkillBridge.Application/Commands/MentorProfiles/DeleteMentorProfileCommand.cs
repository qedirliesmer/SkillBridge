using MediatR;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.MentorProfiles;

public record DeleteMentorProfileCommand(int Id, int UserId) : IRequest<bool>;

public class DeleteMentorProfileCommandHandler : IRequestHandler<DeleteMentorProfileCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMentorProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteMentorProfileCommand request, CancellationToken cancellationToken)
    {
        var mentorProfile = await _unitOfWork.MentorProfiles.GetMentorWithDetailsAsync(request.Id, cancellationToken);

        if (mentorProfile == null)
        {
            throw new KeyNotFoundException("Mentor profile not found.");
        }

        if (mentorProfile.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this profile.");
        }

        var hasActiveBookings = mentorProfile.Bookings.Any(b =>
            b.ScheduledDate > DateTime.UtcNow &&
            (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending));

        if (hasActiveBookings)
        {
            throw new InvalidOperationException("Action Denied: You have upcoming active bookings. Please complete or cancel your appointments before deleting your profile.");
        }

        _unitOfWork.MentorProfiles.Delete(mentorProfile);

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}