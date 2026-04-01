using MediatR;
using SkillBridge.Application.Common.Models;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.MentorProfiles;

public record GetMentorsPagedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SkillName = null,
    int? MinExperience = null) : IRequest<PagedResult<MentorProfileListDto>>;

