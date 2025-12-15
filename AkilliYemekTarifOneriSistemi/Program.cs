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

            // ===============================
            // ?? DATABASE CONNECTION
            // ===============================
            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            Console.WriteLine(">>> DefaultConnection = " + connectionString);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // ===============================
            // ?? IDENTITY (Admin / User)
            // ===============================
            builder.Services
                .AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // ===============================
            // ?? CONTROLLERS + JSON
            // ===============================
            builder.Services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    o.JsonSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddRazorPages();

            // ===============================
            // ?? SWAGGER
            // ===============================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ===============================
            // ?? CORS (React)
            // ===============================
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

            // ===============================
            // ?? SERVICES (DI)
            // ===============================
            builder.Services.AddScoped<INutritionService, NutritionService>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();
            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();
            builder.Services.AddScoped<IRecommendationService, RecommendationService>();
            builder.Services.AddScoped<IHealthProfileService, HealthProfileService>();
            builder.Services.AddScoped<IAllergyService, AllergyService>();
            builder.Services.AddScoped<IWeeklyPlanService, WeeklyPlanService>();

            var app = builder.Build();

            // ===============================
            // ?? SEED (Admin Role + User)
            // ===============================
            using (var scope = app.Services.CreateScope())
            {
                await AdminSeed.SeedAdminAsync(scope.ServiceProvider);
            }

            // ===============================
            // ?? MIDDLEWARE PIPELINE
            // ===============================
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

            // ===============================
            // ??? ROUTES
            // ===============================
            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
