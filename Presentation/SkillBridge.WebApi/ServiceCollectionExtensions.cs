using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.Options;
using SkillBridge.Domain.Constants;
using SkillBridge.Domain.Entities;
using SkillBridge.Infrastructure.Persistence.Context;
using SkillBridge.Infrastructure.Services;
using SkillBridge.WebApi.Options;
using System.Text;
namespace SkillBridge.WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<SkillBridgeDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                         ?? throw new InvalidOperationException("JwtOptions section is missing in appsettings.json");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        // 6.4 Authorization (SkillBridge biznes məntiqinə uyğun yeniləndi)
        services.AddAuthorization(options =>
        {
            // Sənəddəki ManageCities -> SkillBridge-də sistem ayarları/kateqoriyalar üçün
            options.AddPolicy(Policies.ManageCategories, p => p.RequireRole(RoleNames.Admin));

            //// Mentor və Adminlərin kurs idarə etməsi üçün
            options.AddPolicy(Policies.ManageCourses, p => p.RequireRole(RoleNames.Admin, RoleNames.Mentor));

            //// Hər bir giriş etmiş istifadəçi üçün
            options.AddPolicy(Policies.ManageProfile, p => p.RequireAuthenticatedUser());
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SkillBridge API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // Sənəd tələbi: Kiçik hərflə "bearer"
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddValidatorsFromAssemblyContaining<IAuthService>();

        return services;
    }
}
