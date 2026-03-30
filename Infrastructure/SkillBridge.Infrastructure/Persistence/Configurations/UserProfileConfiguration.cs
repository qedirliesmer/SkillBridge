using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Bio)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(u => u.LinkedInUrl)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(u => u.TimeZone)
            .IsRequired()
            .HasDefaultValue("UTC+4")
            .HasMaxLength(50);

        builder.HasOne(u => u.MentorProfile)
            .WithOne()
            .HasForeignKey<MentorProfile>(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.StudentInterests)
            .WithOne(si => si.Student)
            .HasForeignKey(si => si.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.Student)
            .HasForeignKey(b => b.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.ReviewsGiven)
            .WithOne(r => r.FromUserProfile)
            .HasForeignKey(r => r.FromUserProfileId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.SentMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.ReceivedMessages)
            .WithOne(m => m.Receiver)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(u => new { u.FirstName, u.LastName });
    }
}
