namespace SkillBridge.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseApiPipeline(this IApplicationBuilder app)
    { 

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
