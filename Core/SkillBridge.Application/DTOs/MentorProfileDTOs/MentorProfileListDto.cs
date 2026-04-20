using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.MentorProfileDTOs;

public class MentorProfileListDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string MentorFullName { get; set; } = null!;
    public string CurrentJobTitle { get; set; } = null!;
    public string Company { get; set; } = null!;
    public int YearsOfExperience { get; set; }
    public decimal Rating { get; set; }
    public List<string> TopSkills { get; set; } = new();
}

