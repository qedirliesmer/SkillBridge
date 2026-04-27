using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Interfaces;
using SkillBridge.Domain.Entities;
using SkillBridge.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Repositories;

public class SkillRepository : GenericRepository<Skill>, ISkillRepository
{
    public SkillRepository(SkillBridgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Skill>> GetSkillsWithCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .Include(s => s.Category)
            .Include(s => s.MentorSkills) 
            .Include(s => s.StudentInterests) 
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
