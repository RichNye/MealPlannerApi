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

            // Configure the database connection
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContext<ApplicationDbContext>
                (options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            }
            else
            {
                builder.Services.AddDbContext<ApplicationDbContext>
                (options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
