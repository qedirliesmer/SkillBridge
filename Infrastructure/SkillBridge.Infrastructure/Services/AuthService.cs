using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.DTOs.AuthDTOs;
using SkillBridge.Application.Options;
using SkillBridge.Domain.Constants;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailService _emailService; 
    private readonly IOptions<EmailOptions> _emailOptions;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService,
        IEmailService emailService, 
        IOptions<EmailOptions> emailOptions,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _jwtOptions = jwtOptions.Value;
        _emailService = emailService;
        _emailOptions = emailOptions;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var nameParts = (request.FullName ?? string.Empty).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : "User";
        var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Registration failed.";
            return (false, error);
        }

        await _userManager.AddToRoleAsync(user, RoleNames.User);

        try
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_emailOptions.Value.ConfirmationBaseUrl}?token={Uri.EscapeDataString(token)}&email={user.Email}";

            await _emailService.SendEmailAsync(
                user.Email!,
                "Confirm Your Account",
                $"Welcome to our platform! Please <a href='{confirmationLink}'>click here</a> to verify your email address and complete your registration.");
        }
        catch (Exception)
        {
        }

        return (true, null);
    }
    public async Task<(bool Success, string? Error)> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, "User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return (false, "The confirmation link is invalid or has expired.");
        }

        return (true, null);
    }

    public async Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Login);

        if (user == null) return null;

        if (!user.EmailConfirmed)
        {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded) return null;

        return await BuildTokenResponseAsync(user);
    }

    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        var user = await _refreshTokenService.ValidateAndConsumeAsync(refreshToken);

        if (user == null) return null;

        return await BuildTokenResponseAsync(user);
    }

    private async Task<TokenResponse> BuildTokenResponseAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, roles);

        var refreshToken = await _refreshTokenService.CreateAsync(user);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes)
        };
    }
}
