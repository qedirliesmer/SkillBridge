using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.DTOs.AuthDTOs;
using SkillBridge.Application.Models;
using SkillBridge.Domain.Constants;

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

        return Ok(BaseResponse.Success("Registration successful. Please check your email to confirm your account."));
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        // Giriş parametrlərinin ilkin yoxlanışı
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(BaseResponse.Fail("Email and token are required."));
        }

        var (success, error) = await _authService.ConfirmEmailAsync(email, token);

        if (!success)
        {
            return BadRequest(BaseResponse.Fail(error ?? "Email confirmation failed."));
        }

        return Ok(BaseResponse.Success("Email confirmed successfully. You can now log in."));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(request, ct);

        if (response == null)
        {
            return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid credentials or unconfirmed account."));
        }

        return Ok(BaseResponse<TokenResponse>.Success(response, "Login successful."));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(BaseResponse<TokenResponse>.Fail("Refresh token is required."));
        }

        var response = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (response == null)
        {
            return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid or expired refresh token."));
        }

        return Ok(BaseResponse<TokenResponse>.Success(response, "Token refreshed successfully."));
    }
}
