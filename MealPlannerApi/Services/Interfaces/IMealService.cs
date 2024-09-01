using MealPlannerApi.Models;

namespace MealPlannerApi.Services.Interfaces
{
    public interface IMealService
    {
        public List<Meal> GetRandomMeals(int noOfMeals);
    }
}
