using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class Booking:BaseEntity
{
    public int MentorId { get; set; }
    public  int StudentId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public  TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public BookingStatus Status {  get; set; }
    public string? MeetingLink { get; set; }
    public virtual MentorProfile Mentor { get; set; } = null!;
    public virtual UserProfile Student { get; set; } = null!;
    public virtual Review? Review { get; set; }
}
