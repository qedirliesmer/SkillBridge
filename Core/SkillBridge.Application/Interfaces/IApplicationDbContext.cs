using Microsoft.EntityFrameworkCore;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<MentorProfile> MentorProfiles { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}