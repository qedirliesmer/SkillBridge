using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.SkillDTOs;
using SkillBridge.Application.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Skills;

public class GetSkillByIdQueryHandler : IRequestHandler<GetSkillByIdQuery, IResult<SkillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSkillByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<SkillDto>> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
    {
        var skill = await _unitOfWork.Skills
            .GetWhere(s => s.Id == request.Id)
            .Include(s => s.Category)
            .Include(s => s.MediaItems)
            .AsNoTracking() 
            .FirstOrDefaultAsync(cancellationToken);

        if (skill == null)
            return Result<SkillDto>.Failure("Skill not found.");

        var dto = _mapper.Map<SkillDto>(skill);
        return Result<SkillDto>.Success(dto);
    }
}
