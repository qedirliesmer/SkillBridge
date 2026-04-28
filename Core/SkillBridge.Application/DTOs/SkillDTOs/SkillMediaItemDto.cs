using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.SkillDTOs;

public class SkillMediaItemDto
{
    public int Id { get; set; }
    public string ObjectKey { get; set; } = null!;
    public int Order { get; set; }
}