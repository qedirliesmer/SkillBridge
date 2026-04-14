using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SkillBridge.Application.Options;
using SkillBridge.Domain.Entities;
using SkillBridge.Infrastructure.Persistence.Data;

namespace SkillBridge.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static async Task UseApiPipelineAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await RoleSeeder.SeedAsync(roleManager);

            if (app.Environment.IsDevelopment())
            {
                var userManager = services.GetRequiredService<UserManager<User>>();
                var seedOptions = services.GetRequiredService<IOptions<SeedOptions>>();
                await AdminSeeder.SeedAsync(userManager, seedOptions);
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillBridge API v1"));
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }

    public static void UseApiPipeline(this WebApplication app)
    {
        app.UseApiPipelineAsync().GetAwaiter().GetResult();
    }
}
