using MediatR;
using SkillBridge.Application.DTOs.StudentInterestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.UserProfileQueries;

public record GetStudentInterestsQuery(string UserId) : IRequest<List<InterestDto>>;
