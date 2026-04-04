using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Application.Commands.MentorProfiles;
using SkillBridge.Application.Queries.MentorProfiles;

namespace SkillBridge.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MentorProfilesController : ControllerBase
{
    private readonly IMediator _mediator;
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMentorProfileCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = id }, id);
    }
    public MentorProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetMentorsPagedQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRated([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetTopRatedMentorsQuery(count));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetMentorProfileByIdQuery(id));
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateMentorProfileCommand command)
    {
        await _mediator.Send(command);
        return NoContent(); 
    }

    [HttpPut("skills")]
    public async Task<IActionResult> UpdateSkills([FromBody] UpdateMentorSkillsCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteMentorProfileCommand(id, 1));
        return NoContent();
    }
}
