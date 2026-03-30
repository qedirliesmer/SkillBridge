using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Domain.Entities;

public class UserProfile : BaseEntity
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string TimeZone { get; set; }= "UTC+4";
    public virtual MentorProfile? MentorProfile { get; set; }
    public virtual ICollection<StudentInterest> StudentInterests { get; set; } = new HashSet<StudentInterest>();
    public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    public virtual ICollection<Review> ReviewsGiven { get; set; } = new HashSet<Review>();
    public virtual ICollection<MatchHistory> MatchHistories { get; set; } = new HashSet<MatchHistory>();
    public virtual ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();
    public virtual ICollection<Message> ReceivedMessages { get; set; } = new HashSet<Message>();
}
