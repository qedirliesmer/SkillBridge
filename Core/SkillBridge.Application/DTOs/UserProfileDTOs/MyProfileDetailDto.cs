using SkillBridge.Application.DTOs.MentorProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.UserProfileDTOs;

public class MyProfileDetailDto
{
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string TimeZone { get; set; } = "Asia/Baku";

    public List<string> Interests { get; set; } = new();

    public bool IsMentor { get; set; }
    public MentorSummaryDto? MentorInfo { get; set; }
}