using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.Property(b => b.Status)
            .IsRequired();

        builder.Property(b => b.MeetingLink)
            .HasMaxLength(500)
            .IsRequired(false); 

        builder.Property(b => b.ScheduledDate)
            .IsRequired();

        builder.HasOne(b => b.Mentor)
            .WithMany(m => m.Bookings)
            .HasForeignKey(b => b.MentorId)
            .OnDelete(DeleteBehavior.NoAction); 

        builder.HasOne(b => b.Student)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.StudentId)
            .OnDelete(DeleteBehavior.NoAction); 

        builder.HasOne(b => b.Review)
            .WithOne(r => r.Booking)
            .HasForeignKey<Review>(r => r.BookingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.ToTable(t => t.HasCheckConstraint("CK_Booking_TimeRange", "\"StartTime\" < \"EndTime\""));
    }
}
