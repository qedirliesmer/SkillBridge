using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface IUserProfileRepository : IGenericRepository<UserProfile>
{
    Task<UserProfile?> GetProfileWithDetailsAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetPublicProfileAsync(string userId, CancellationToken cancellationToken = default);
}