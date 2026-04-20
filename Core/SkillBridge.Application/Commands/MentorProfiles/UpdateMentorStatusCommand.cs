using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SkillBridge.Application.Commands.MentorProfiles;

public class UpdateMentorStatusCommand : IRequest<Unit>
{
    public int MentorProfileId { get; set; }
    public MentorStatus Status { get; set; }

    public UpdateMentorStatusCommand(UpdateMentorStatusDto dto)
    {
        MentorProfileId = dto.MentorProfileId;
        Status = (MentorStatus)dto.Status;
    }
}
public class UpdateMentorStatusCommandHandler : IRequestHandler<UpdateMentorStatusCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateMentorStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateMentorStatusCommand request, CancellationToken cancellationToken)
    {
        var mentor = await _context.MentorProfiles
            .FirstOrDefaultAsync(m => m.Id == request.MentorProfileId, cancellationToken);

        if (mentor == null)
        {
            throw new Exception("Mentor profile not found.");
        }

        mentor.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
