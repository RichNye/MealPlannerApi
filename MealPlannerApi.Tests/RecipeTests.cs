using MealPlannerApi.Models;
using NuGet.ContentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerApi.Tests
{
    public class RecipeTests
    {
        [Fact]
        public void Recipe_can_have_a_name()
        {
            var recipe = new Recipe
            {
                Name = "Spaghetti Bolognese"
            };

            Asset.Equals("Spaghetti Bolognese", recipe.Name);
        }
    }
}
