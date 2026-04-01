using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.MentorProfileDTOs;

public class MentorProfileDetailDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int YearsOfExperience { get; set; }
    public string CurrentJobTitle { get; set; } = null!;
    public string Company { get; set; } = null!;
    public decimal Rating { get; set; }
    public List<string> Skills { get; set; } = new();
    public decimal AverageRating { get; set; }
    public List<MentorProfileReviewDto> Reviews { get; set; } = new();
}
