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

public class AvailabilityRepository : GenericRepository<Availability>, IAvailabilityRepository
{
    public AvailabilityRepository(SkillBridgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Availability>> GetByMentorIdAsync(int mentorId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Availability>()
            .AsNoTracking() 
            .Where(x => x.MentorId == mentorId)
            .OrderBy(x => (int)x.DayOfWeek) 
            .ThenBy(x => x.StartTime)     
            .ToListAsync(cancellationToken);
    }
    public async Task<bool> IsSlotAvailableAsync(int mentorId, Days dayOfWeek, TimeSpan startTime, TimeSpan endTime, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Availability>()
            .AsNoTracking()
            .AnyAsync(x => x.MentorId == mentorId &&
                           x.DayOfWeek == dayOfWeek &&
                           x.StartTime <= startTime &&
                           x.EndTime >= endTime,
                           cancellationToken);
    }
    public async Task<bool> HasOverlappingSlotAsync(int mentorId, Days dayOfWeek, TimeSpan startTime, TimeSpan endTime, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Availability>()
            .AsNoTracking()
            .Where(x => x.MentorId == mentorId && x.DayOfWeek == dayOfWeek);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(x =>
            x.StartTime < endTime && startTime < x.EndTime, cancellationToken);
    }
}


