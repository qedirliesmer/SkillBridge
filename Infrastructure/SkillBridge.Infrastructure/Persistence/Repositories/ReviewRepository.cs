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

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(SkillBridgeDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetReviewsByMentorIdAsync(int mentorId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.FromUserProfile)
                .ThenInclude(u => u.User) 
            .Where(r => r.ToMentorProfileId == mentorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetReviewWithDetailsAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.FromUserProfile).ThenInclude(u => u.User)
            .Include(r => r.ToMentorProfile).ThenInclude(m => m.User)
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
    }
}