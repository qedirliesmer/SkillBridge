using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface IMentorProfileRepository : IGenericRepository<MentorProfile>
{
    Task<MentorProfile?> GetMentorWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<MentorProfile>> GetMentorsBySkillAsync(string skillName, CancellationToken cancellationToken = default);
}
