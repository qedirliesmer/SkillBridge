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

public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(SkillBridgeDbContext context) : base(context)
    {
    }
    public async Task<UserProfile?> GetProfileWithDetailsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Include(up => up.User)                  
            .Include(up => up.StudentInterests)     
                .ThenInclude(si => si.Skill)        
            .Include(up => up.MentorProfile)     
            .FirstOrDefaultAsync(up => up.UserId == userId, cancellationToken);
    }
    public async Task<UserProfile?> GetPublicProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .AsNoTracking()                    
            .Include(up => up.User)
            .Include(up => up.StudentInterests)
                .ThenInclude(si => si.Skill)
            .FirstOrDefaultAsync(up => up.UserId == userId, cancellationToken);
    }
}
