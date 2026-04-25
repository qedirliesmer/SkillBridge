using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<bool> HasConflictAsync(int mentorId, DateTime date, TimeSpan start, TimeSpan end, CancellationToken ct = default);
    Task<Booking?> GetBookingWithDetailsAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId, bool isMentor, CancellationToken ct = default);
}
