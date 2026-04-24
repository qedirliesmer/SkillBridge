using MediatR;
using SkillBridge.Application.DTOs.UserProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.UserProfileQueries;

public record GetUserProfileQuery(string UserId) : IRequest<MyProfileDetailDto>;
