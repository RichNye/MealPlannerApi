using MealPlannerApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using MealPlannerApi.Data;
using MealPlannerApi.Services.Interfaces;
using MealPlannerApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace MealPlannerApi.Controllers
{
    [Authorize]
    [Route("api/meals")]
    [ApiController]
    public class MealController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MealController(ApplicationDbContext context) 
        { 
            _context= context;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] MealWithoutMealPlanDTO mealDTO)
        {
            Meal meal = new Meal();

            // check the provided mealDTO matches validation in the model. Return 400 if it doesn't.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            meal.Recipe = await _context.Recipes.FindAsync(mealDTO.RecipeId);
            if (meal == null)
            {
                return BadRequest(ModelState);
            }
            else if(meal.Recipe != null)
            {
                meal.RecipeId = mealDTO.RecipeId;
                meal.MealDate = mealDTO.MealDate;
                meal.MealName = meal.Recipe.Name;
                meal.MealPlanId = null;
                meal.MealPlan = null;

                try
                {
                    _context.Meals.Add(meal);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(503, ex);
                }

            }

            return CreatedAtAction(nameof(GetById), new { id = meal.Id }, meal);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            Meal meal = await _context.Meals.FindAsync(id);

            if (meal == null)
            {
                return NotFound($"Meal with id {id} could not be found.");
            }

            MealWithoutMealPlanDTO mealDTO = new MealWithoutMealPlanDTO
            {
                MealDate = meal.MealDate,
                RecipeId = meal.RecipeId
            };
            
            return Ok(mealDTO);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] MealWithoutMealPlanDTO mealDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Meal meal = await _context.Meals.FindAsync(id);
                if (meal == null)
                {
                    return NotFound();
                }

                meal.MealDate = mealDTO.MealDate;
                meal.RecipeId = mealDTO.RecipeId;

                int result = await _context.SaveChangesAsync();

                if (result == 0)
                {
                    return BadRequest("No changes made");
                }
                else
                {
                    return Ok(meal);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            Meal meal = await _context.Meals.FindAsync(id);
            if (meal != null)
            {
                try
                {
                    _context.Meals.Remove(meal);
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
    }
}
