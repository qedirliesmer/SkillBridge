using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetCategoryWithSkillsAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Category>> GetCategoriesWithSkillsAsync(CancellationToken cancellationToken = default);
}