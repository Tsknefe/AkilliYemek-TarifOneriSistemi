using AkilliYemekTarifOneriSistemi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AkilliYemekTarifOneriSistemi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>() // 🔥 ROLE EKLENDİ
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();


            // 🌟 TEST USER + ADMIN ROLE SEED
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string adminEmail = "admin@example.com";
                string password = "Admin123!";

                // 1) ADMIN ROLE VAR MI?
                bool adminRoleExists = await roleManager.RoleExistsAsync("Admin");
                if (!adminRoleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // 2) ADMIN USER VAR MI?
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(adminUser, password);
                }

                // 3) ADMIN ROLE VER
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }


                // ⭐ BONUS: normal test user da kalsın istersen
                string testEmail = "testuser@example.com";
                string testPassword = "Test123!";

                var testUser = await userManager.FindByEmailAsync(testEmail);
                if (testUser == null)
                {
                    testUser = new IdentityUser
                    {
                        UserName = testEmail,
                        Email = testEmail,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(testUser, testPassword);
                }
            }
            // 🌟 SEED BİTTİ


            // Configure request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication(); // 🔥 OLMAZSA LOGIN ÇALIŞMAZ
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages().WithStaticAssets();

            app.Run();
        }
    }
}
