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

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(SkillBridgeDbContext context) : base(context) { }

    public async Task<bool> HasConflictAsync(int mentorId, DateTime date, TimeSpan start, TimeSpan end, CancellationToken ct = default)
    {
        return await _context.Set<Booking>().AnyAsync(b =>
            b.MentorId == mentorId &&
            b.ScheduledDate.Date == date.Date &&
            b.Status != BookingStatus.Cancelled &&
            ((start >= b.StartTime && start < b.EndTime) || 
             (end > b.StartTime && end <= b.EndTime) ||    
             (start <= b.StartTime && end >= b.EndTime)),  
            ct);
    }

    public async Task<Booking?> GetBookingWithDetailsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<Booking>()
            .Include(b => b.Mentor)
                .ThenInclude(m => m.User) 
            .Include(b => b.Student)
                .ThenInclude(s => s.User) 
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId, bool isMentor, CancellationToken ct = default)
    {
        var query = _context.Set<Booking>()
            .Include(b => isMentor ? b.Student.User : b.Mentor.User)
            .AsQueryable();

        if (isMentor)
            query = query.Where(b => b.MentorId == userId);
        else
            query = query.Where(b => b.StudentId == userId);

        return await query
            .OrderByDescending(b => b.ScheduledDate)
            .ThenByDescending(b => b.StartTime)
            .ToListAsync(ct);
    }
}