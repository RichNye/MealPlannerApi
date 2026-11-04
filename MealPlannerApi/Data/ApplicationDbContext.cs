using MealPlannerApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        { }

        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<Meal> Meals { get; set; }

        public DbSet<MealPlan> MealPlans { get; set; }
    }
}
