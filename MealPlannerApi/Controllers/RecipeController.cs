using MealPlannerApi.Models;
using Microsoft.AspNetCore.Mvc;
using MealPlannerApi.Data;
using MealPlannerApi.Services.Interfaces;
using MealPlannerApi.Models.DTOs;

namespace RecipePlannerApi.Controllers
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
        public async Task<IActionResult> Get()
        {
            Console.WriteLine($"Caller: {HttpContext.Connection.RemoteIpAddress}");
            List<Recipe> Recipes = _recipeService.GetRandomRecipes(7);
            return Ok(Recipes);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody]RecipeBasicDTO recipeBasic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Recipe recipe = await _context.Recipes.FindAsync(id);
                if (recipe == null)
                {
                    return NotFound();
                }
                
                recipe.Description = recipeBasic.Description;
                recipe.Name = recipeBasic.Name;

                int result = await _context.SaveChangesAsync();

                if (result == 0)
                {
                    return BadRequest("No changes made");
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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            Console.WriteLine($"Caller: {HttpContext.Connection.RemoteIpAddress}");
            try
            {
                Recipe? recipe = await _context.Recipes.FindAsync(id);

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
        public async Task<IActionResult> Add([FromBody] RecipeBasicDTO recipeDTO)
        {

            // instantiate a new Recipe object to populate with the DTO fields
            Recipe recipe = new Recipe();

            // check the provided RecipeDTO matches validation in the model. Return 400 if it doesn't.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // model must be valid if we've hit this point. Populate the full Recipe object.
            recipe.CreatedDate = DateTime.UtcNow;
            recipe.Name = recipeDTO.Name;
            if (recipeDTO.Description != null)
            {
                recipe.Description = recipeDTO.Description;
            }

            try
            {
                // add the Recipe and sync changes with the actual DB.
                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
            }
            catch (Exception ex)
            {
                // log the error. Strip the exception out if production.
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while saving the Recipe: {ex}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            Recipe? recipe = _context.Recipes.Find(id);
            if (recipe != null)
            {
                try
                {
                    _context.Recipes.Remove(recipe);
                    await _context.SaveChangesAsync();
                    return Ok("Recipe deleted successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                return NotFound("Recipe not found with that id.");
            }
        }

        [HttpDelete("delete/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                List<Recipe> recipes = _context.Recipes.ToList();
                foreach(var recipe in recipes)
                {
                    _context.Remove(recipe);
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
