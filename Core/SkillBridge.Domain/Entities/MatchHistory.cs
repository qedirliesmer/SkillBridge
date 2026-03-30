using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class MatchHistory:BaseEntity
{
    public int StudentId { get; set; }
    public int MentorId { get; set; }
    public decimal MatchScore { get; set; }
    public virtual UserProfile Student { get; set; } = null!;
    public virtual MentorProfile Mentor { get; set; } = null!;
}
