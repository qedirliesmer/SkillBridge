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
using SkillBridge.Application.Abstracts.Services;
using Microsoft.AspNetCore.Identity;
using SkillBridge.Domain.Entities;

namespace SkillBridge.Application.Commands.MentorProfiles;

public class UpdateMentorStatusCommand : IRequest<Unit>
{
    public int MentorProfileId { get; set; }
    public MentorStatus Status { get; set; }
    public string? RejectReason { get; set; }

    public UpdateMentorStatusCommand(UpdateMentorStatusDto dto)
    {
        MentorProfileId = dto.MentorProfileId;
        Status = (MentorStatus)dto.Status;
        RejectReason = dto.RejectReason;
    }

    public UpdateMentorStatusCommand() { }
}

public class UpdateMentorStatusCommandHandler : IRequestHandler<UpdateMentorStatusCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;

    public UpdateMentorStatusCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        UserManager<User> userManager)
    {
        _context = context;
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task<Unit> Handle(UpdateMentorStatusCommand request, CancellationToken cancellationToken)
    {
        var mentor = await _context.MentorProfiles
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == request.MentorProfileId, cancellationToken);

        if (mentor == null)
        {
            throw new Exception("Mentor profile not found.");
        }

        if (request.Status == MentorStatus.Rejected && string.IsNullOrWhiteSpace(request.RejectReason))
        {
            throw new Exception("A rejection reason must be provided when rejecting an application.");
        }

        mentor.Status = request.Status;

        if (request.Status == MentorStatus.Rejected)
        {
            mentor.RejectReason = request.RejectReason;
        }
        else if (request.Status == MentorStatus.Approved)
        {
            mentor.RejectReason = null; 

            if (mentor.User != null)
            {
                var roles = await _userManager.GetRolesAsync(mentor.User);
                if (!roles.Contains("Mentor"))
                {
                    var roleResult = await _userManager.AddToRoleAsync(mentor.User, "Mentor");
                    if (!roleResult.Succeeded)
                    {
                        var errorMsg = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to assign Mentor role: {errorMsg}");
                    }
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (mentor.User != null && !string.IsNullOrEmpty(mentor.User.Email))
        {
            try
            {
                await SendNotificationEmail(
                    mentor.User.Email,
                    mentor.User.FullName,
                    request.Status,
                    request.RejectReason);
            }
            catch (Exception)
            {
                // Burada loglama (Logger) istifadə etmək tövsiyə olunur
            }
        }

        return Unit.Value;
    }

    private async Task SendNotificationEmail(string email, string fullName, MentorStatus status, string? reason)
    {
        string subject = status == MentorStatus.Approved
            ? "Your Mentorship Application has been Approved! 🎉"
            : "Update regarding your Mentorship Application";

        string statusText = status == MentorStatus.Approved ? "Approved" : "Rejected";

        string body = $@"
                <div style='font-family: sans-serif; line-height: 1.6;'>
                    <h3>Hello {fullName},</h3>
                    <p>Your mentorship application has been reviewed. Status: <strong>{statusText}</strong></p>
                    {(status == MentorStatus.Rejected && !string.IsNullOrEmpty(reason) ? $"<p><strong>Reason for rejection:</strong> {reason}</p>" : "")}
                    <p>Best regards,<br/>SkillBridge Team</p>
                </div>";

        await _emailService.SendEmailAsync(email, subject, body);
    }
}