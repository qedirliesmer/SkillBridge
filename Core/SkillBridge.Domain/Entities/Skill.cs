using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class Skill:BaseEntity
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<MentorSkill> MentorSkills { get; set; } = new HashSet<MentorSkill>();
    public virtual ICollection<StudentInterest> StudentInterests { get; set; } = new HashSet<StudentInterest>();

    public ICollection<SkillMedia> MediaItems { get; set; } = new List<SkillMedia>();
}
