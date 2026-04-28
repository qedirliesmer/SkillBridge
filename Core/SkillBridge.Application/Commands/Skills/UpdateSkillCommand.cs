using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.SkillDTOs;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Commands.Skills;

public record UpdateSkillCommand(UpdateSkillDto Dto) : IRequest<IResult<Unit>>;

public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, IResult<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _storageService;
    private readonly IDistributedCache _cache; 

    public UpdateSkillCommandHandler(IUnitOfWork unitOfWork, IFileStorageService storageService, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _cache = cache;
    }

    public async Task<IResult<Unit>> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _unitOfWork.Skills.GetWhere(s => s.Id == request.Dto.Id)
            .Include(s => s.MediaItems)
            .FirstOrDefaultAsync(cancellationToken);

        if (skill == null) return Result<Unit>.Failure("Skill not found.");

        if (request.Dto.RemoveMediaIds != null && request.Dto.RemoveMediaIds.Any())
        {
            var mediasToRemove = skill.MediaItems
                .Where(m => request.Dto.RemoveMediaIds.Contains(m.Id)).ToList();

            foreach (var media in mediasToRemove)
            {
                await _storageService.DeleteFileAsync(media.ObjectKey, cancellationToken);
                skill.MediaItems.Remove(media);
            }
        }

        if (request.Dto.NewImages != null && request.Dto.NewImages.Any())
        {
            int lastOrder = skill.MediaItems.Any() ? skill.MediaItems.Max(m => m.Order) : 0;

            foreach (var file in request.Dto.NewImages)
            {
                using var stream = file.OpenReadStream();
                var objectKey = await _storageService.SaveAsync(stream, file.FileName, file.ContentType, skill.Id, cancellationToken);

                skill.MediaItems.Add(new SkillMedia
                {
                    ObjectKey = objectKey,
                    Order = ++lastOrder,
                    SkillId = skill.Id
                });
            }
        }

        skill.Name = request.Dto.Name;
        skill.CategoryId = request.Dto.CategoryId;

        _unitOfWork.Skills.Update(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync("all_skills_stats", cancellationToken);
        await _cache.RemoveAsync($"skill_{skill.Id}", cancellationToken);

        return Result<Unit>.Success(Unit.Value, "Skill updated successfully with media.");
    }
}