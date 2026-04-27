using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Application.Commands.Skills;
using SkillBridge.Application.DTOs.SkillDTOs;
using SkillBridge.Application.Queries.Skills;
using SkillBridge.Domain.Constants;

namespace SkillBridge.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkillsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSkillsWithStats()
    {
        var result = await _mediator.Send(new GetSkillsWithStatsQuery());
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetSkillByIdQuery(id));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create([FromBody] CreateSkillDto dto)
    {
        var result = await _mediator.Send(new CreateSkillCommand(dto));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update([FromBody] UpdateSkillDto dto)
    {
        var result = await _mediator.Send(new UpdateSkillCommand(dto));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteSkillCommand(id));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
