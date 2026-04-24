using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.UserProfileDTOs;

public class PublicUserProfileDto
{
    public string FullName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public List<string> Interests { get; set; } = new();
}