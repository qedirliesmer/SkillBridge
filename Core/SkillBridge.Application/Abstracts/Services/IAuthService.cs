using SkillBridge.Application.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Abstracts.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
    Task<(bool Success, string? Error)> ConfirmEmailAsync(string email, string token);
}
