using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AkilliYemekTarifOneriSistemi.Data.Seed
{
    public static class AdminSeed
    {
        public static async Task SeedAdminAsync(IServiceProvider service)
        {
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = service.GetRequiredService<UserManager<IdentityUser>>();

            
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            
            var adminEmail = "admin@akilliyemek.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");

            }
            await SeedIngredientsAsync(service);

        }
        private static async Task SeedIngredientsAsync(IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            
            if (await db.Ingredients.AnyAsync())
                return;

            db.Ingredients.AddRange(
                new Ingredient { Name = "Süt" },
                new Ingredient { Name = "Yumurta" },
                new Ingredient { Name = "Un" },
                new Ingredient { Name = "Fýstýk" },
                new Ingredient { Name = "Gluten" },
                new Ingredient { Name = "Domates" },
                new Ingredient { Name = "Tavuk" }
            );

            await db.SaveChangesAsync();
        }
    }
}



