using graduated_project.Models;
using Microsoft.AspNetCore.Identity;

namespace graduated_project.Services
{
    public static class DbInitializer
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider, IConfiguration config)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            var roleName = "super admin";

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var email = config["SuperAdmin:Email"];
            var password = config["SuperAdmin:Password"];
            var username = config["SuperAdmin:Username"];
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
                    throw new Exception(
                        string.Join(" | ", result.Errors.Select(e => e.Description))
                    );
                }
            }

            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
