using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Application.Commands.Availabilities;
using SkillBridge.Application.DTOs.AvaiabilityDTOs;
using SkillBridge.Application.Queries.Availabilities;
using SkillBridge.Domain.Constants;

namespace SkillBridge.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AvailabilitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AvailabilitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = Policies.MentorOrAdmin)]
    public async Task<IActionResult> Create([FromBody] CreateAvailabilityDto dto)
    {
        var result = await _mediator.Send(new CreateAvailabilityCommand(dto));

        if (result.IsSuccess)
            return Ok(new { Message = "The availability slot has been successfully created.", Data = result.Data });

        return BadRequest(new { Message = result.Message });
    }

    [HttpPut]
    [Authorize(Policy = Policies.MentorOrAdmin)]
    public async Task<IActionResult> Update([FromBody] UpdateAvailabilityDto dto)
    {
        var result = await _mediator.Send(new UpdateAvailabilityCommand(dto));

        if (result.IsSuccess)
            return Ok(new { Message = "The availability slot has been successfully updated." });

        return BadRequest(new { Message = result.Message });
    }
    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.MentorOrAdmin)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteAvailabilityCommand(id));

        if (result.IsSuccess)
            return Ok(new { Message = "The availability slot has been successfully removed." });

        return BadRequest(new { Message = result.Message });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetAvailabilityByIdQuery(id));

        if (result == null)
            return NotFound(new { Message = "Requested availability slot could not be found." });

        return Ok(result);
    }

    [HttpGet("mentor/{mentorId}")]
    public async Task<IActionResult> GetByMentorId(int mentorId)
    {
        var result = await _mediator.Send(new GetMentorAvailabilitiesQuery(mentorId));
        return Ok(result);
    }
}