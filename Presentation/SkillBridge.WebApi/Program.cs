using SkillBridge.Application;
using SkillBridge.Infrastructure;
using SkillBridge.WebApi;
using SkillBridge.WebApi.Extensions;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.Run();