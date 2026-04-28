using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Skills;

public record DeleteSkillCommand(int Id) : IRequest<IResult<Unit>>;

public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, IResult<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _storageService;
    private readonly IDistributedCache _cache; 

    public DeleteSkillCommandHandler(IUnitOfWork unitOfWork, IFileStorageService storageService, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _cache = cache;
    }

    public async Task<IResult<Unit>> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _unitOfWork.Skills.GetWhere(s => s.Id == request.Id)
            .Include(s => s.MediaItems)
            .FirstOrDefaultAsync(cancellationToken);

        if (skill == null) return Result<Unit>.Failure("Skill not found.");

        if (skill.MediaItems != null && skill.MediaItems.Any())
        {
            foreach (var media in skill.MediaItems)
            {
                await _storageService.DeleteFileAsync(media.ObjectKey, cancellationToken);
            }
        }

        _unitOfWork.Skills.Delete(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync("all_skills_stats", cancellationToken);
        await _cache.RemoveAsync($"skill_{request.Id}", cancellationToken);

        return Result<Unit>.Success(Unit.Value, "Skill and all associated media files deleted successfully.");
    }
}