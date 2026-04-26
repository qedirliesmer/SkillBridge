using MediatR;
using SkillBridge.Application.DTOs.AvaiabilityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.Queries.Availabilities;

public record GetAvailabilityByIdQuery(int Id) : IRequest<AvailabilityDto>;