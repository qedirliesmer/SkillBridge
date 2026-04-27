using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.ReviewDTOs;

public class ReviewDetailDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string StudentName { get; set; } = null!;
    public string MentorName { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}