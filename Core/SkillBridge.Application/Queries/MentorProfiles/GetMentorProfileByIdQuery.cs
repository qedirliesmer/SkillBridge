using MediatR;
using SkillBridge.Application.DTOs.MentorProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.MentorProfiles;

public record GetMentorProfileByIdQuery(int Id) : IRequest<MentorProfileDetailDto>;