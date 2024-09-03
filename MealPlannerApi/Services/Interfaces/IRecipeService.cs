using MealPlannerApi.Models;

namespace MealPlannerApi.Services.Interfaces
{
    public interface IRecipeService
    {
        public List<Recipe> GetRandomRecipes(int noOfRecipes);
    }
}
