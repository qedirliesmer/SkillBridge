using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class MentorProfile:BaseEntity
{
    public string UserId { get; set; } = null!;
    public int YearsOfExperience { get; set; }
    public string CurrentJobTitle { get; set; } = null!;
    public string Company { get; set; }=null!;
    public decimal Rating { get; set; } = 0.00m;
    public virtual User User { get; set; } = null!;
    public MentorStatus Status { get; set; } = MentorStatus.Pending;
    public virtual ICollection<MentorSkill> MentorSkills { get; set; } = new HashSet<MentorSkill>();
    public virtual ICollection<Availability> Availabilities { get; set; } = new HashSet<Availability>();
    public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    public virtual ICollection<Review> ReviewsReceived { get; set; } = new HashSet<Review>();
    public virtual ICollection<MatchHistory> MatchHistories { get; set; } = new HashSet<MatchHistory>();
}
