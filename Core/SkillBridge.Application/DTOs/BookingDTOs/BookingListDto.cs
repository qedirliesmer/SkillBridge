using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.BookingDTOs;

public record BookingListDto
{
    public int Id { get; init; }
    public string PartnerFullName { get; init; } = null!; 
    public string PartnerJobTitleOrBio { get; init; } = null!;
    public DateTime ScheduledDate { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string Status { get; init; } = null!;
    public string? MeetingLink { get; init; }
}