using AkilliYemekTarifOneriSistemi.Data;
using AkilliYemekTarifOneriSistemi.Data.Seed;
using AkilliYemekTarifOneriSistemi.Services.Implementations;
using AkilliYemekTarifOneriSistemi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace AkilliYemekTarifOneriSistemi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            
            
            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            Console.WriteLine(">>> DefaultConnection = " + connectionString);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            
            
            
            builder.Services
                .AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            
            
            
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    o.JsonSerializerOptions.WriteIndented = true;
                });

            
            builder.Services.AddRazorPages();

            
            
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            
            
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactDevClient", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            
            
            
            builder.Services.AddScoped<INutritionService, NutritionService>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();
            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();
            builder.Services.AddScoped<IRecommendationService, RecommendationService>();
            builder.Services.AddScoped<IHealthProfileService, HealthProfileService>();
            builder.Services.AddScoped<IAllergyService, AllergyService>();
            builder.Services.AddScoped<IWeeklyPlanService, WeeklyPlanService>();


            var app = builder.Build();

            
            
            
            using (var scope = app.Services.CreateScope())
            {
                await AdminSeed.SeedAdminAsync(scope.ServiceProvider);

                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await TestDataSeed.SeedAsync(db);
            }

            
            
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("ReactDevClient");

            app.UseAuthentication();
            app.UseAuthorization();

            
            
            

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
