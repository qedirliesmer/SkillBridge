using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class MentorProfileConfiguration : IEntityTypeConfiguration<MentorProfile>
{
    public void Configure(EntityTypeBuilder<MentorProfile> builder)
    {
        builder.ToTable("MentorProfiles");

        builder.Property(m => m.CurrentJobTitle)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.Company)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.YearsOfExperience)
            .IsRequired();

        builder.Property(m => m.Rating)
            .HasPrecision(3, 2)
            .HasDefaultValue(0.00m);

        builder.HasOne(m => m.User)
           .WithOne(u => u.MentorProfile)
           .HasForeignKey<MentorProfile>(m => m.UserId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Availabilities)
            .WithOne(a => a.Mentor)
            .HasForeignKey(a => a.MentorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Bookings)
            .WithOne(b => b.Mentor)
            .HasForeignKey(b => b.MentorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(m => m.ReviewsReceived)
            .WithOne(r => r.ToMentorProfile)
            .HasForeignKey(r => r.ToMentorProfileId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(m => m.Rating);

        builder.ToTable(t => t.HasCheckConstraint("CK_Mentor_Experience", "\"YearsOfExperience\" >= 0"));
    }
}
