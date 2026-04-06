using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.Interfaces;
using SkillBridge.Application.Options;
using SkillBridge.Application.UnitOfWork;
using SkillBridge.Domain.Entities;
using SkillBridge.Infrastructure.Persistence.Context;
using SkillBridge.Infrastructure.Persistence.Repositories;
using SkillBridge.Infrastructure.Persistence.UnitOfWork;
using SkillBridge.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SkillBridgeDbContext>(options =>
             options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IMentorProfileRepository, MentorProfileRepository>();
    }
}
