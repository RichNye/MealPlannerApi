using MealPlannerApi.Data;
using MealPlannerApi.Services;
using MealPlannerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Azure.Monitor.OpenTelemetry.AspNetCore;

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

            // OpenTelemetry for Azure
            builder.Services.AddOpenTelemetry().UseAzureMonitor();

            // Configure the database connection
            if (builder.Environment.IsDevelopment())
            {
                string? environmentDB = "Postgres"; //Environment.GetEnvironmentVariable("MEALPLANNER_DB");
                switch(environmentDB)
                {
                    case "SQLServer":
                        builder.Services.AddDbContext<ApplicationDbContext>
                            (options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));
                        break;
                    case "Postgres":
                        builder.Services.AddDbContext<ApplicationDbContext>
                            (options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
                        break;
                    case null:
                        throw new Exception("MEALPLANNER_DB environment variable not set.");
                }
            }
            else
            {
                var connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
                builder.Services.AddDbContext<ApplicationDbContext>(options => 
                    options.UseSqlServer(connection));
            }
            var app = builder.Build();

            // Configure the HTTP request pipeline.
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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
