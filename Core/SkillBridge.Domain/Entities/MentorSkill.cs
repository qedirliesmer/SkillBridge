using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class MentorSkill
{
    public int SkillId { get; set; }
    public SkillLevel Level { get; set; }
    public int MentorId { get; set; }
    public virtual MentorProfile Mentor { get; set; } = null!;
    public virtual Skill Skill { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
