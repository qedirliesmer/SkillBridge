using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.DTOs.AuthDTOs;
using SkillBridge.Application.Models;

namespace SkillBridge.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var (success, error) = await _authService.RegisterAsync(request, ct);

        if (!success)
        {
            return BadRequest(BaseResponse.Fail(error));
        }

        return Ok(BaseResponse.Success("User registered successfully."));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var token = await _authService.LoginAsync(request, ct);

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(BaseResponse<string>.Fail("Invalid login or password."));
        }

        return Ok(BaseResponse<string>.Success(token));
    }
}
