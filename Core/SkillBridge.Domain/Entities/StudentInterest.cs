using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class StudentInterest:BaseEntity
{
    public int StudentId { get; set; }
    public int SkillId { get; set; }
    public virtual UserProfile Student { get; set; } = null!;
    public virtual Skill Skill { get; set; } = null!;
}
