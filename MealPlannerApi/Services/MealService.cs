using MealPlannerApi.Data;
using MealPlannerApi.Models;
using MealPlannerApi.Services.Interfaces;

namespace MealPlannerApi.Services
{
    public class MealService : IRecipeService
    {
        private readonly ApplicationDbContext _context;

        public MealService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Recipe> GetRandomRecipes(int numberOfRecipes)
        {
            Console.WriteLine("Service GetRandomRecipes method called.")
            List<Recipe> userRecipes = _context.Recipes.OrderBy(m => Guid.NewGuid()).Take(numberOfRecipes).ToList<Recipe>();

            return userRecipes;
        }
    }
}
