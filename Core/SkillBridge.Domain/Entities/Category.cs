using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class Category:BaseEntity
{
    public string Name { get; set; } = null!;
    public virtual ICollection<Skill> Skills { get; set; } = new HashSet<Skill>();
}
