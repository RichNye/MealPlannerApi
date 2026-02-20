using MealPlannerApi.Data;
using MealPlannerApi.Services;
using MealPlannerApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
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

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("FrontendPolicy", policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:8090")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
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
