using SkillBridge.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.UnitOfWork;

public interface IUnitOfWork:IDisposable
{
    IMentorProfileRepository MentorProfiles { get; }
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
