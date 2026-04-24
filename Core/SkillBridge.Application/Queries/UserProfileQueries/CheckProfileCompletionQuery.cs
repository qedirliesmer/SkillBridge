using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.UserProfileQueries;

public class ProfileCompletionDto
{
    public bool IsComplete { get; set; }
    public int CompletionPercentage { get; set; }
    public List<string> MissingSteps { get; set; } = new();
}

public record CheckProfileCompletionQuery(string UserId) : IRequest<ProfileCompletionDto>;