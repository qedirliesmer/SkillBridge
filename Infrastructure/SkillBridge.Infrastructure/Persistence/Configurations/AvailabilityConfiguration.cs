using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
{
    public void Configure(EntityTypeBuilder<Availability> builder)
    {
        builder.ToTable("Availabilities");

        builder.Property(a => a.MentorId)
            .IsRequired();

        builder.Property(a => a.StartTime).HasColumnType("time").IsRequired();
        builder.Property(a => a.EndTime).HasColumnType("time").IsRequired();

        builder.Property(a => a.DayOfWeek)
            .IsRequired();

        builder.HasOne(a => a.Mentor)
            .WithMany(m => m.Availabilities)
            .HasForeignKey(a => a.MentorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(t => t.HasCheckConstraint("CK_Availability_TimeRange", "\"StartTime\" < \"EndTime\""));
    }
}
