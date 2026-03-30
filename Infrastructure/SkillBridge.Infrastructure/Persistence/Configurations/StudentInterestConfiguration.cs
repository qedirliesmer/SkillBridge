using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class StudentInterestConfiguration : IEntityTypeConfiguration<StudentInterest>
{
    public void Configure(EntityTypeBuilder<StudentInterest> builder)
    {
        builder.ToTable("StudentInterests");

        builder.HasKey(si => new { si.StudentId, si.SkillId });

        builder.HasOne(si => si.Student)
            .WithMany(u => u.StudentInterests)
            .HasForeignKey(si => si.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(si => si.Skill)
            .WithMany(s => s.StudentInterests)
            .HasForeignKey(si => si.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
