using graduated_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace graduated_project.Services
{
    public static class DbInitializer
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider, IConfiguration config)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Ensure the basic roles exist
            var roles = new[] { "super admin", "admin", "user" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var email = config["SuperAdmin:Email"];
            var password = config["SuperAdmin:Password"];
            var username = config["SuperAdmin:Username"];

            // If required config is missing, don't attempt to create the super admin
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var firstName = config["SuperAdmin:FirstName"];
            var lastName = config["SuperAdmin:LastName"];
            var address = config["SuperAdmin:Address"];
            var mobilnum = config["SuperAdmin:Mobilnum"];

            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    Mobilnum = mobilnum
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(" | ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(user, "super admin"))
            {
                await userManager.AddToRoleAsync(user, "super admin");
            }
        }
    }
}
