using Microsoft.EntityFrameworkCore;

namespace MealPlannerApi.Models
{
    public class MealContext : DbContext
    {
        public DbSet<Meal> Meals { get; set; }
        public MealContext(DbContextOptions<MealContext> options) : base(options) { }


    }
}
