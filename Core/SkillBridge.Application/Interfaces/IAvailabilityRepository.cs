using SkillBridge.Domain.Entities;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface IAvailabilityRepository : IGenericRepository<Availability>
{
    Task<IEnumerable<Availability>> GetByMentorIdAsync(int mentorId, CancellationToken cancellationToken = default);
    Task<bool> IsSlotAvailableAsync(int mentorId, Days dayOfWeek, TimeSpan startTime, TimeSpan endTime, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingSlotAsync(int mentorId, Days dayOfWeek, TimeSpan startTime, TimeSpan endTime, int? excludeId = null, CancellationToken cancellationToken = default);
}
