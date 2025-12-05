using graduated_project.Models;
using graduated_project.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using Microsoft.Extensions.Logging;
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
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IAppUserProductRepository, AppUserProductRepository>();
            builder.Services.AddIdentity<AppUser,IdentityRole>().AddEntityFrameworkStores<ShopSpheredbcontext>();

            var app = builder.Build();

            // Log some startup information to help diagnose hosting issues
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting application. Environment: {env}. Listening URLs: {urls}", app.Environment.EnvironmentName, urlsEnv ?? "http://0.0.0.0:5000");

            // تطبيق المايجريشن وإنشاء Super Admin
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var services = scope.ServiceProvider;
                    var config = services.GetRequiredService<IConfiguration>();
                    var context = services.GetRequiredService<ShopSpheredbcontext>();

                    DbInitializer.SeedSuperAdminAsync(services, config)
                                 .GetAwaiter()
                                 .GetResult();

                    logger.LogInformation("Database initialization completed successfully.");
                }
                catch (Exception ex)
                {
                    // Log the error without stopping the application
                    logger.LogError(ex, "Database initialization error");
                    Console.WriteLine($"⚠️ Database Error: {ex.Message}");
                    Console.WriteLine($"⚠️ Stack Trace: {ex.StackTrace}");
                    Console.WriteLine("🔄 التطبيق سيعمل بدون قاعدة بيانات...");
                }
            }

            // Configure the HTTP request pipeline.
            var useHttpsRedirection = builder.Configuration.GetValue<bool?>("UseHttpsRedirection") ?? true;

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                if (useHttpsRedirection)
                {
                    logger.LogInformation("UseHttpsRedirection is enabled.");
                    app.UseHttpsRedirection();
                }
                else
                {
                    logger.LogInformation("UseHttpsRedirection is disabled by configuration.");
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
