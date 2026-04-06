using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Context;

public class SkillBridgeDbContext:IdentityDbContext<User>
{
    public SkillBridgeDbContext(DbContextOptions<SkillBridgeDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<MentorProfile> MentorProfiles { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<MentorSkill> MentorSkills { get; set; }
    public DbSet<StudentInterest> StudentInterests { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MatchHistory> MatchHistories { get; set; }
    public DbSet<RefreshToken> RefreshTokens {  get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow; // BaseEntity-də varsa açarsan
            }

            entity.UpdatedAt = DateTime.UtcNow; // BaseEntity-də varsa açarsan
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
