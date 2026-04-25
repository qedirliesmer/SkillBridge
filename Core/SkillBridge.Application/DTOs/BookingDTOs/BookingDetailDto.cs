using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.BookingDTOs;

public record BookingDetailDto
{
    public int Id { get; init; }
    public int MentorId { get; init; }
    public string MentorFullName { get; init; } = null!;
    public string MentorJobTitle { get; init; } = null!;
    public string MentorCompany { get; init; } = null!;
    public int StudentId { get; init; }
    public string StudentFullName { get; init; } = null!;
    public string? StudentProfilePicture { get; init; }
    public DateTime ScheduledDate { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string Status { get; init; } = null!;
    public string? MeetingLink { get; init; }
    public bool IsReviewed { get; init; }
    public double? ReviewRating { get; init; }
    public string? ReviewComment { get; init; }
    public bool IsCurrentUserMentor { get; set; }
}
