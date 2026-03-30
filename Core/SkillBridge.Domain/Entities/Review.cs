using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class Review:BaseEntity
{
    public int BookingId { get; set; }
    public int FromUserProfileId { get; set; }
    public int ToMentorProfileId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public virtual Booking Booking { get; set; } = null!;
    public virtual UserProfile FromUserProfile { get; set; } = null!;
    public virtual MentorProfile ToMentorProfile { get; set; } = null!;
}
