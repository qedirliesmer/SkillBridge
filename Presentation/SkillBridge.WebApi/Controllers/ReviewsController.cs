using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Application.Commands.Reviews;
using SkillBridge.Application.DTOs.ReviewDTOs;
using SkillBridge.Application.Queries.Reviews;
using SkillBridge.Domain.Constants;

namespace SkillBridge.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = Policies.ManageProfile)]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
        var result = await _mediator.Send(new CreateReviewCommand(dto));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("mentor/{mentorId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByMentorId(int mentorId)
    {
        var result = await _mediator.Send(new GetMentorReviewsQuery(mentorId));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = Policies.MentorOrAdmin)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetReviewDetailQuery(id));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteReviewCommand(id));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}