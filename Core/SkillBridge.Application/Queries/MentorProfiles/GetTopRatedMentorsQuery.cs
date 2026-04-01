using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.MentorProfiles;

public record GetTopRatedMentorsQuery(int Count = 5) : IRequest<IEnumerable<MentorProfileListDto>>;