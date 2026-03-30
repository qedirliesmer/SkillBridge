using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasMany(c => c.Skills)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
