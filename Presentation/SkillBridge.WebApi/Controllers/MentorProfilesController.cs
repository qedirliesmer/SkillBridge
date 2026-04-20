using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Application.Commands.MentorProfiles;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using SkillBridge.Application.Queries.MentorProfiles;
using SkillBridge.Domain.Constants;
using System.Security.Claims;

namespace SkillBridge.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MentorProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MentorProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("{id}/status")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateMentorStatusDto statusDto)
    {
        if (id != statusDto.MentorProfileId)
        {
            return BadRequest(new { Message = "ID mismatch occurred." });
        }

        try
        {
            await _mediator.Send(new UpdateMentorStatusCommand(statusDto));

            return Ok(new { Message = "Mentor status updated successfully and notification email sent." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateMentorProfileCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = id }, new
        {
            Id = id,
            Message = "Mentor profile created successfully. Please wait for admin approval."
        });
    }

    [HttpGet("admin/all")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetAllForAdmin([FromQuery] GetMentorsPagedQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(new { Data = result, Message = "All mentor profiles retrieved for administration." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetMentorsPagedQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(new { Data = result, Message = "Mentor profiles retrieved successfully." });
    }

    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRated([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetTopRatedMentorsQuery(count));
        return Ok(new { Data = result, Message = "Top rated mentors retrieved successfully." });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetMentorProfileByIdQuery(id));
        if (result == null) return NotFound(new { Message = "Mentor profile not found." });
        return Ok(new { Data = result, Message = "Mentor profile details retrieved successfully." });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Policies.MentorOrAdmin)]
    public async Task<IActionResult> Update(int id, [FromBody] MentorProfileUpdateDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest(new { Message = "ID mismatch occurred." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _mediator.Send(new UpdateMentorProfileCommand(updateDto, userId, isAdmin));
            return Ok(new { Message = "Mentor profile updated successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/skills")]
    [Authorize(Policy = Policies.MentorOrAdmin)]
    public async Task<IActionResult> UpdateSkills(int id, [FromBody] MentorProfileUpdateSkillsDto skillsDto)
    {
        if (id != skillsDto.MentorProfileId)
            return BadRequest(new { Message = "Mentor ID mismatch occurred." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _mediator.Send(new UpdateMentorSkillsCommand(skillsDto, userId, isAdmin));
            return Ok(new { Message = "Mentor skills updated successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.MentorOrAdmin)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var isAdmin = User.IsInRole("Admin");

        var result = await _mediator.Send(new DeleteMentorProfileCommand(id, userId, isAdmin));

        if (!result) return NotFound(new { Message = "Mentor profile not found." });
        return Ok(new { Message = "Mentor profile deleted successfully." });
    }
}
