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

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(SkillBridgeDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetCategoryWithSkillsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithSkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Skills)
            .ToListAsync(cancellationToken);
    }
}