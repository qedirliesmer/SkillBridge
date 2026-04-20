using MediatR;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;              
using SkillBridge.Application.Interfaces; 
using SkillBridge.Domain.Enums;                 
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.MentorProfiles;

public record DeleteMentorProfileCommand(int Id, string UserId, bool IsAdmin) : IRequest<bool>;

public class DeleteMentorProfileHandler : IRequestHandler<DeleteMentorProfileCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteMentorProfileHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteMentorProfileCommand request, CancellationToken cancellationToken)
    {
        var mentorProfile = await _unitOfWork.MentorProfiles.GetMentorWithDetailsAsync(request.Id, cancellationToken);
        if (mentorProfile == null) return false;

        if (mentorProfile.UserId != request.UserId && !request.IsAdmin)
        {
            throw new UnauthorizedAccessException("Siz yalnız özünüzə aid mentor profilini silə bilərsiniz.");
        }

        bool hasActiveBookings = mentorProfile.Bookings?.Any(b => b.Status == BookingStatus.Confirmed) ?? false;
        if (hasActiveBookings)
            throw new InvalidOperationException("Bu mentorun təsdiqlənmiş görüşləri var.");

        _unitOfWork.Repository<MentorProfile>().Delete(mentorProfile);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}