using Microsoft.Extensions.DependencyInjection;
using SkillBridge.Application.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
namespace SkillBridge.Application;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddMaps(Assembly.GetExecutingAssembly());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
    }
}