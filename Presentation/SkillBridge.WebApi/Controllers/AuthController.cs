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
            return BadRequest(BaseResponse.Fail(error ?? "An error occurred during registration."));
        }

        return Ok(BaseResponse.Success("User registered successfully."));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(request, ct);

        if (response == null)
        {
            return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid email or password."));
        }

        return Ok(BaseResponse<TokenResponse>.Success(response));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(BaseResponse<TokenResponse>.Fail("Refresh token must be provided."));
        }

        var response = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (response == null)
        {
            return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid or expired refresh token."));
        }

        return Ok(BaseResponse<TokenResponse>.Success(response));
    }
}