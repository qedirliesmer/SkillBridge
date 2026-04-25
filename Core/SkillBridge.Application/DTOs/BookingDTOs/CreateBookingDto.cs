using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.BookingDTOs;

public record CreateBookingDto(
    int MentorId,
    DateTime ScheduledDate,
    TimeSpan StartTime,
    TimeSpan EndTime
);

