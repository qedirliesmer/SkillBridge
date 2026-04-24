using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Commands.UserProfiles;
using SkillBridge.Application.DTOs.UserProfileDTOs;
using SkillBridge.Application.Queries.UserProfileQueries;
using SkillBridge.Domain.Constants;
using System.Security.Claims;

namespace SkillBridge.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class UserProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Personal Profile Management (ManageProfile Policy)


    [Authorize(Policy = Policies.ManageProfile)]
    [HttpGet("my-profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _mediator.Send(new GetUserProfileQuery(userId));
        return Ok(result);
    }

    [Authorize(Policy = Policies.ManageProfile)]
    [HttpGet("completion-status")]
    public async Task<IActionResult> GetCompletionStatus()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _mediator.Send(new CheckProfileCompletionQuery(userId));
        return Ok(result);
    }

    [Authorize(Policy = Policies.ManageProfile)]
    [HttpGet("interests")]
    public async Task<IActionResult> GetInterests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _mediator.Send(new GetStudentInterestsQuery(userId));
        return Ok(result);
    }

    [Authorize(Policy = Policies.ManageProfile)]
    [HttpPost("create")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateUserProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profileId = await _mediator.Send(new CreateUserProfileCommand(dto, userId));
        return CreatedAtAction(nameof(GetMyProfile), new { id = profileId }, profileId);
    }

    [Authorize(Policy = Policies.ManageProfile)]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _mediator.Send(new UpdateUserProfileCommand(dto, userId));
        return NoContent();
    }

    #endregion

    #region Administrative & Mentor Access (Privileged Policies)

    [Authorize(Policy = Policies.MentorOrAdmin)]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProfileById(string userId)
    {
        var result = await _mediator.Send(new GetUserProfileQuery(userId));
        return Ok(result);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteProfile(string userId)
    {
        await _mediator.Send(new DeleteUserProfileCommand(userId));
        return NoContent();
    }
    #endregion
}

