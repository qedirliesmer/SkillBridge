using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Interfaces;
using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
using SkillBridge.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Repositories;

public class MentorProfileRepository : GenericRepository<MentorProfile>, IMentorProfileRepository
{
    public MentorProfileRepository(SkillBridgeDbContext context) : base(context)
    {
    }

    public async Task<MentorProfile?> GetMentorWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.MentorProfiles
            .AsNoTracking()
            .Include(m => m.User)
            .Include(m => m.MentorSkills)
                .ThenInclude(ms => ms.Skill)
            .Include(m => m.ReviewsReceived)
                .ThenInclude(r => r.FromUserProfile)
            .Include(m => m.Availabilities)
            .Include(m => m.Bookings)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MentorProfile>> GetMentorsBySkillAsync(string skillName, CancellationToken cancellationToken = default)
    {
        return await _context.MentorProfiles
            .AsNoTracking()
            .Include(m => m.User)
            .Include(m => m.MentorSkills)
                .ThenInclude(ms => ms.Skill)
            .Where(m => m.Status == MentorStatus.Approved)
            .Where(m => m.MentorSkills.Any(ms => ms.Skill.Name.ToLower() == skillName.ToLower()))
            .ToListAsync(cancellationToken);
    }
}