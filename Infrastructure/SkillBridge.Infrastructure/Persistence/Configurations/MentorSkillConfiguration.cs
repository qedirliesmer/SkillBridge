using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class MentorSkillConfiguration : IEntityTypeConfiguration<MentorSkill>
{
    public void Configure(EntityTypeBuilder<MentorSkill> builder)
    {
        builder.ToTable("MentorSkills");

        builder.HasKey(ms => new { ms.MentorId, ms.SkillId });

        builder.Property(ms => ms.Level)
            .IsRequired();

        builder.HasOne(ms => ms.Mentor)
            .WithMany(m => m.MentorSkills)
            .HasForeignKey(ms => ms.MentorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ms => ms.Skill)
            .WithMany(s => s.MentorSkills)
            .HasForeignKey(ms => ms.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
