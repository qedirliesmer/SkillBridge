using Microsoft.EntityFrameworkCore;
using SkillBridge.Infrastructure.Persistence.Context;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SkillBridgeDbContext>(options =>
    options.UseNpgsql(connectionString));
// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
