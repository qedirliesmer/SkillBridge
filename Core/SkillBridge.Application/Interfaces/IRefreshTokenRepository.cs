using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenWithUserAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    Task DeleteByTokenAsync(string token);
}
