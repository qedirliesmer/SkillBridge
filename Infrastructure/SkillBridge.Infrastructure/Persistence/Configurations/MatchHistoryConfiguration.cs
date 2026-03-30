using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class MatchHistoryConfiguration : IEntityTypeConfiguration<MatchHistory>
{
    public void Configure(EntityTypeBuilder<MatchHistory> builder)
    {
        builder.ToTable("MatchHistories");

        builder.Property(mh => mh.MatchScore)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.HasOne(mh => mh.Student)
            .WithMany(u => u.MatchHistories)
            .HasForeignKey(mh => mh.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(mh => mh.Mentor)
            .WithMany(m => m.MatchHistories)
            .HasForeignKey(mh => mh.MentorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(mh => new { mh.StudentId, mh.MentorId }).IsUnique();

        builder.HasIndex(mh => mh.MatchScore);
    }
}
