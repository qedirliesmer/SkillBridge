using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Application.Commands.Bookings;
using SkillBridge.Application.Queries.Bookings;
using SkillBridge.Domain.Constants;

namespace SkillBridge.WebApi.Controllers;

[Authorize] 
[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my-requests")]
    public async Task<ActionResult> GetMyBookings([FromQuery] GetStudentBookingsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(new
        {
            Message = "Your booking requests have been loaded successfully.",
            Data = result
        });
    }

    [Authorize(Policy = Policies.MentorOrAdmin)] 
    [HttpGet("mentor-appointments")]
    public async Task<ActionResult> GetMentorAppointments([FromQuery] GetMentorBookingsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(new
        {
            Message = "Appointment requests assigned to you have been retrieved.",
            Data = result
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetBookingDetail(int id)
    {
        var result = await _mediator.Send(new GetBookingDetailQuery(id));
        return Ok(new
        {
            Message = "Detailed information for the booking has been retrieved.",
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateBookingCommand command)
    {
        var bookingId = await _mediator.Send(command);
        return Ok(new
        {
            Message = "Your booking request has been sent to the mentor. Status: Pending.",
            BookingId = bookingId
        });
    }

    [Authorize(Policy = Policies.MentorOrAdmin)]
    [HttpPut("{id}/confirm")]
    public async Task<ActionResult> Confirm(int id)
    {
        await _mediator.Send(new ConfirmBookingCommand(id));
        return Ok(new { Message = "Booking has been confirmed. The session is now scheduled." });
    }

    [Authorize(Policy = Policies.MentorOrAdmin)]
    [HttpPut("{id}/complete")]
    public async Task<ActionResult> Complete(int id)
    {
        await _mediator.Send(new CompleteBookingCommand(id));
        return Ok(new { Message = "Booking marked as completed. Feedback can now be provided." });
    }

    [HttpDelete("{id}/cancel")]
    public async Task<ActionResult> Cancel(int id, [FromBody] string reason)
    {
        await _mediator.Send(new CancelBookingCommand(id, reason));
        return Ok(new { Message = "The booking has been successfully cancelled." });
    }
}