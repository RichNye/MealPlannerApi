using MealPlannerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MealPlannerApi.Data;
using MealPlannerApi.Services.Interfaces;
using MealPlannerApi.Models.DTOs;

namespace MealPlannerApi.Controllers
{
    [Route("api/recipes")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IRecipeService _recipeService;


        public RecipeController(ApplicationDbContext context, IRecipeService recipeService)
        {
            _context = context;
            _recipeService = recipeService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Recipe> Get()
        {
            List<Recipe> meals = _recipeService.GetRandomRecipes(7);
            return Ok(meals);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Recipe> GetById(int id)
        {
            try
            {
                Recipe? recipe = _context.Recipes.Find(id);

                if (recipe == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(recipe);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] RecipeBasicDTO mealDTO)
        {

            // instantiate a new meal object to populate with the DTO fields
            Recipe recipe = new Recipe();

            // check the provided mealDTO matches validation in the model. Return 400 if it doesn't.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // model must be valid if we've hit this point. Populate the full meal object.
            recipe.CreatedDate = DateTime.UtcNow;
            recipe.Name = mealDTO.Name;
            if (mealDTO.Description != null)
            {
                recipe.Description = mealDTO.Description;
            }

            try
            {
                // add the meal and sync changes with the actual DB.
                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
            }
            catch (Exception ex)
            {
                // log the error. Strip the exception out if production.
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while saving the meal: {ex}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            Recipe? meal = _context.Recipes.Find(id);
            if (meal != null)
            {
                try
                {
                    _context.Recipes.Remove(meal);
                    await _context.SaveChangesAsync();
                    return Ok("meal deleted successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                return NotFound("Meal not found with that id.");
            }
        }

        [HttpDelete("delete/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                List<Recipe> meals = _context.Recipes.ToList();
                foreach(var meal in meals)
                {
                    _context.Remove(meal);
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }            
        }
    }
}
