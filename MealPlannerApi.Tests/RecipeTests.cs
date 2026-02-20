using MealPlannerApi.Models;
using Xunit;
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

            Assert.Equal("Spaghetti Bolognese", recipe.Name);
        }

        [Fact]
        public void Recipe_can_have_a_description()
        {
            var recipe = new Recipe
            {
                Description = "A classic Italian pasta dish with a rich meat sauce."
            };
            Assert.Equal("A classic Italian pasta dish with a rich meat sauce.", recipe.Description);
        }
    }
}
