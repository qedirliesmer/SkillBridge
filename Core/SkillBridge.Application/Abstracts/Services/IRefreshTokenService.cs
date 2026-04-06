using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Abstracts.Services;
public interface IRefreshTokenService
{
    Task<string> CreateAsync(User user);
    Task<User?> ValidateAndConsumeAsync(string token);
}
