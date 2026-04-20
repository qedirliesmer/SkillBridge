using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.MentorProfileDTOs;

public class MentorProfileReviewDto
{
    public string StudentName { get; set; } = null!;
    public string? StudentProfilePictureUrl { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
