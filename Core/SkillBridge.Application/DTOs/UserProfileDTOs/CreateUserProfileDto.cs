using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.UserProfileDTOs;

public class CreateUserProfileDto
{
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string TimeZone { get; set; } = "Asia/Baku";
    public List<int> InterestIds { get; set; } = new(); 
}