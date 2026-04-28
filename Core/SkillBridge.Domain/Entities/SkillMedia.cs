using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class SkillMedia : BaseEntity
{
    public string ObjectKey { get; set; } = default!;

    public int Order { get; set; }

    public int SkillId { get; set; }

    public Skill Skill { get; set; } = default!;
}