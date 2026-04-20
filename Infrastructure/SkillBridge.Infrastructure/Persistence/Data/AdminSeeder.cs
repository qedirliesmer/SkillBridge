using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SkillBridge.Application.Options;
using SkillBridge.Domain.Constants;
using SkillBridge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(UserManager<User> userManager, IOptions<SeedOptions> seedOptions)
    {
        var options = seedOptions.Value;

        if (string.IsNullOrEmpty(options.AdminEmail) ||
            string.IsNullOrEmpty(options.AdminPassword))
        {
            return;
        }

        var existingAdmin = await userManager.FindByEmailAsync(options.AdminEmail);
        if (existingAdmin != null) return;

        var adminUser = new User
        {
            UserName = options.AdminEmail,
            Email = options.AdminEmail,
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true
        };

        if (!string.IsNullOrEmpty(options.AdminFullName))
        {
            var names = options.AdminFullName.Split(' ');
            adminUser.FirstName = names[0];
            adminUser.LastName = names.Length > 1 ? names[1] : "";
        }

        var result = await userManager.CreateAsync(adminUser, options.AdminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
        }
    }
}