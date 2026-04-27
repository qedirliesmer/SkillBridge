using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface ISkillRepository : IGenericRepository<Skill>
{
    Task<IEnumerable<Skill>> GetSkillsWithCategoriesAsync(CancellationToken cancellationToken = default);
}
