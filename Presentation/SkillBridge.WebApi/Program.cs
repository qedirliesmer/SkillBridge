using Microsoft.EntityFrameworkCore;
using SkillBridge.Infrastructure;
using SkillBridge.Infrastructure.Persistence.Context;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
