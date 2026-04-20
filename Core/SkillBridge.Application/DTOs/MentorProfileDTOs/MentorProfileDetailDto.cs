using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.MentorProfileDTOs;

public class MentorProfileDetailDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string MentorFullName { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public int YearsOfExperience { get; set; }
    public string CurrentJobTitle { get; set; } = null!;
    public string Company { get; set; } = null!;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<MentorProfileReviewDto> Reviews { get; set; } = new();
}
