using Microsoft.Extensions.Options;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.Interfaces;
using SkillBridge.Application.Options;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtOptions _jwtOptions;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtOptions> jwtOptions)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<string> CreateAsync(User user)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        string token = Convert.ToBase64String(randomNumber);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = user.Id,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshExpirationMinutes)
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        return token;
    }

    public async Task<User?> ValidateAndConsumeAsync(string token)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenWithUserAsync(token);

        if (storedToken == null || storedToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return null;
        }

        await _refreshTokenRepository.DeleteByTokenAsync(token);

        return storedToken.User;
    }
}