using MealPlannerApi.Data;
using MealPlannerApi.Services;
using MealPlannerApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace MealPlannerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(o =>
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Adds the new MealService for business logic for Meals
            builder.Services.AddScoped<IRecipeService, MealService>();

            // add Identity and authorization
            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Configure the database connection
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContext<ApplicationDbContext>
                (options => options.UseNpgsql(builder.Configuration.GetConnectionString("DevelopmentConnection")));

                // Allow any origin in dev — SetIsOriginAllowed is required instead of AllowAnyOrigin
                // because AllowAnyOrigin() and AllowCredentials() cannot be used together.
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("FrontendPolicy", policy =>
                    {
                        policy
                            .SetIsOriginAllowed(_ => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });

                // In dev, the frontend and API are on different origins, so the browser rejects
                // SameSite=Lax cookies cross-site. Override to SameSite=None with Secure=false
                // so cookies are sent across origins over plain HTTP.
                // Do not use this in production.
                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.Unspecified;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                });
            }
            else
            {
                builder.Services.AddDbContext<ApplicationDbContext>
                (options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            var app = builder.Build();
            app.Logger.LogInformation("App started");
            app.Logger.LogInformation("Environment: {EnvironmentName}", app.Environment.EnvironmentName);
            app.Logger.LogInformation("Database Connection String: {ConnectionString}", builder.Configuration.GetConnectionString(app.Environment.IsDevelopment() ? "DevelopmentConnection" : "DefaultConnection"));
            app.MapIdentityApi<IdentityUser>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
            }

            //app.UseHttpsRedirection();
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("FrontendPolicy");
            }
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
