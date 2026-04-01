using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.MentorProfileDTOs;

public class MentorProfileUpdateSkillsDto
{
    public int MentorProfileId { get; set; }
    public int UserId { get; set; }
    public List<int> SkillIds { get; set; } = new();
}
