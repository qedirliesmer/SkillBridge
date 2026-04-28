using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Skills)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.MentorSkills)
            .WithOne(ms => ms.Skill)
            .HasForeignKey(ms => ms.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.StudentInterests)
            .WithOne(si => si.Skill)
            .HasForeignKey(si => si.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.MediaItems)
        .WithOne(sm => sm.Skill)
        .HasForeignKey(sm => sm.SkillId)
        .OnDelete(DeleteBehavior.Cascade); 
    }
}
