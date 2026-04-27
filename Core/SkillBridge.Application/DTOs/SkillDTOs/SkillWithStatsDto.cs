using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.SkillDTOs;

public class SkillWithStatsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public int MentorCount { get; set; } 
    public int StudentInterestCount { get; set; } 
}
