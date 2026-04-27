using SkillBridge.Application.DTOs.SkillDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.CategoryDTOs;

public class CategoryWithSkillsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalSkillsCount { get; set; } 
    public IEnumerable<SkillDto> Skills { get; set; } = new List<SkillDto>();
}
