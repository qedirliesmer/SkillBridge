using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.MentorProfileDTOs;

public class MentorSummaryDto
{
    public int Id { get; set; } 
    public string CurrentJobTitle { get; set; } = null!;
    public string Company { get; set; } = null!;
    public int YearsOfExperience { get; set; }
    public decimal Rating { get; set; }
    public string Status { get; set; } = null!;
    public string? RejectReason { get; set; }
    public int ReviewCount { get; set; }
    public List<string> Skills { get; set; } = new();
}