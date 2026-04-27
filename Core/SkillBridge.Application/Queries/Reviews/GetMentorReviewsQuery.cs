using MediatR;
using SkillBridge.Application.Common.Interfaces;
using SkillBridge.Application.DTOs.ReviewDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Reviews;

public record GetMentorReviewsQuery(int MentorId) : IRequest<IResult<IEnumerable<MentorProfileReviewDto>>>;
