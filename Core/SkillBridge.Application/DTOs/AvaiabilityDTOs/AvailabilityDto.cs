using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.AvaiabilityDTOs;

public class AvailabilityDto
{
    public int Id { get; set; }
    public int MentorId { get; set; }
    public int DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = null!;
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
}