using SkillBridge.Application;
using SkillBridge.Domain.Constants;
using SkillBridge.Infrastructure;
using SkillBridge.WebApi;
using SkillBridge.WebApi.Extensions;
using SkillBridge.WebApi.Middlewares; 
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

try
{
    Log.Information("SkillBridge tətbiqi başladılır...");

    builder.Services.AddApiServices(builder.Configuration);
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddEndpointsApiExplorer();

    var app = builder.Build();

    app.UseSerilogRequestLogging(); 

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillBridge API V1");
            options.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();

    app.UseApiPipeline();

    app.MapControllers();

    Log.Information("Tətbiq uğurla işə düşdü.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Tətbiq gözlənilmədən dayandı!");
}
finally
{
    Log.CloseAndFlush();
}