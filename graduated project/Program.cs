using graduated_project.Models;
using graduated_project.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using System;

namespace graduated_project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Allow explicit override of URLs (useful in some hosting environments)
            var urlsEnv = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            if (!string.IsNullOrEmpty(urlsEnv))
            {
                builder.WebHost.UseUrls(urlsEnv);
            }
            else
            {
                // Default to listen on all network interfaces (HTTP) so external requests can reach the app
                builder.WebHost.UseUrls("http://0.0.0.0:5000");
            }

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var connectionString = builder.Configuration.GetConnectionString("DBConnection");
            builder.Services.AddDbContext<ShopSpheredbcontext>(options => options.UseSqlServer(connectionString) );

            // Configure Identity but do not relax password rules.
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Allow space and common symbols in usernames
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";

                // Keep password rules unchanged
            })
            .AddEntityFrameworkStores<ShopSpheredbcontext>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IAppUserProductRepository, AppUserProductRepository>();

            var app = builder.Build();

            // Run seed initializer (create roles and super admin) using a scoped provider
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var config = services.GetRequiredService<IConfiguration>();

                    // Call the seed method (synchronous wait)
                    DbInitializer.SeedSuperAdminAsync(services, config).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                // Keep output minimal per request
                Console.WriteLine($"Seed error: {ex.Message}");
            }

            // Configure the HTTP request pipeline.
            var useHttpsRedirection = builder.Configuration.GetValue<bool?>("UseHttpsRedirection") ?? true;

            if (app.Environment.IsDevelopment())
            {
                // Enable developer exception page to see detailed errors during local debugging
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                if (useHttpsRedirection)
                {
                    app.UseHttpsRedirection();
                }
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
