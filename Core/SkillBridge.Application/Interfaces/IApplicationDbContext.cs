using Microsoft.EntityFrameworkCore;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<MentorProfile> MentorProfiles { get; set; }
    DbSet<UserProfile> UserProfiles { get; set; }
    DbSet<Booking> Bookings { get; set; }
    DbSet<Skill> Skills { get; set; }
    DbSet<Availability> Availabilities { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}