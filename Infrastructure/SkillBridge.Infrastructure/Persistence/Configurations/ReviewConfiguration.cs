using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .IsRequired(false);


        builder.HasOne(r => r.Booking)
            .WithOne(b => b.Review)
            .HasForeignKey<Review>(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
       

        builder.HasOne(r => r.FromUserProfile)
            .WithMany(u => u.ReviewsGiven)
            .HasForeignKey(r => r.FromUserProfileId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.ToMentorProfile)
            .WithMany(m => m.ReviewsReceived)
            .HasForeignKey(r => r.ToMentorProfileId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating_Range", "\"Rating\" >= 1 AND \"Rating\" <= 5"));

        builder.HasIndex(r => r.BookingId).IsUnique();
    }
}
