using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class SkillMediaConfiguration : IEntityTypeConfiguration<SkillMedia>
{
    public void Configure(EntityTypeBuilder<SkillMedia> builder)
    {
        builder.ToTable("SkillMedias");

        builder.Property(x => x.ObjectKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.SkillId)
            .IsRequired();

        builder.HasOne(x => x.Skill)
            .WithMany(s => s.MediaItems)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SkillId);
        builder.HasIndex(x => new { x.SkillId, x.Order });
    }
}