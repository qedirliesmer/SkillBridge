using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.MentorProfileDTOs;

public class UpdateMentorStatusDto
{
    public int MentorProfileId { get; set; }
    public int Status { get; set; }
    public string? RejectReason { get; set; }
}