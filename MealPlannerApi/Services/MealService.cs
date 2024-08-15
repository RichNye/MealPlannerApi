using MealPlannerApi.Data;
using MealPlannerApi.Models;

namespace MealPlannerApi.Services
{
    public class MealService
    {
        private readonly ApplicationDbContext _context;

        public MealService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Meal> GetRandomMeals(int numberOfMeals)
        {
            List<Meal> userMeals = _context.Meals.OrderBy(m => Guid.NewGuid()).Take(numberOfMeals).ToList<Meal>();

            return userMeals;
        }
    }
}
