using MealPlannerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Meal> Meals { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    }
}
