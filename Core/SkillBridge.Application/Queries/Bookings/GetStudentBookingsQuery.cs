using MediatR;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.BookingDTOs;
using SkillBridge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Bookings;

public record GetStudentBookingsQuery : IRequest<PaginatedList<BookingListDto>>
{
    public BookingStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}