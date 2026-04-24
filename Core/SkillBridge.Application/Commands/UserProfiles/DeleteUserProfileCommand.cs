using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.UserProfiles;

public record DeleteUserProfileCommand(string UserId) : IRequest<Unit>;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _unitOfWork.Repository<UserProfile>()
            .GetWhere(p => p.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userProfile == null)
        {
            throw new KeyNotFoundException($"User profile not found for UserId: {request.UserId}");
        }

        _unitOfWork.Repository<UserProfile>().Delete(userProfile);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}