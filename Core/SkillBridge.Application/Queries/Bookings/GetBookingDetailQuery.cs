using MediatR;
using SkillBridge.Application.DTOs.BookingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Bookings;

public record GetBookingDetailQuery(int Id) : IRequest<BookingDetailDto>;