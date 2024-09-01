using MealPlannerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MealPlannerApi.Data;
using MealPlannerApi.Services.Interfaces;
using MealPlannerApi.Models.DTOs;

namespace MealPlannerApi.Controllers
{
    [Route("api/meals")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMealService _mealService;


        public MealController(ApplicationDbContext context, IMealService mealService)
        {
            _context = context;
            _mealService = mealService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Meal> Get()
        {
            List<Meal> meals = _mealService.GetRandomMeals(7);
            return Ok(meals);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Meal> GetById(int id)
        {
            try
            {
                Meal? meal = _context.Meals.Find(id);

                if (meal == null)
                {
                    return NotFound();
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] MealBasicDTO mealDTO)
        {

            // instantiate a new meal object to populate with the DTO fields
            Meal meal = new Meal();

            // check the provided mealDTO matches validation in the model. Return 400 if it doesn't.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // model must be valid if we've hit this point. Populate the full meal object.
            meal.CreatedDate = DateTime.UtcNow;
            meal.Name = mealDTO.Name;
            if (mealDTO.Description != null)
            {
                meal.Description = mealDTO.Description;
            }

            try
            {
                // add the meal and sync changes with the actual DB.
                _context.Meals.Add(meal);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = meal.Id }, meal);
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
            Meal? meal = _context.Meals.Find(id);
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

        [HttpDelete("delete/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                List<Meal> meals = _context.Meals.ToList();
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
