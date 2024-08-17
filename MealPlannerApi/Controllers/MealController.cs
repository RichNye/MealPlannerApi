using MealPlannerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MealPlannerApi.Data;
using MealPlannerApi.Services;

namespace MealPlannerApi.Controllers
{
    [Route("api/meals")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MealService _mealService;
        

        public MealController(ApplicationDbContext context, MealService mealService)
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
        public ActionResult<Meal> GetById(Guid id)
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
        public async Task<IActionResult> Add([FromBody] Meal meal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(meal.CreatedDate == default(DateTime))
            {
                meal.CreatedDate = DateTime.UtcNow;
            }

            try
            {
                _context.Meals.Add(meal);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = meal.Id }, meal);
            }
            catch (Exception ex)
            {
                // Optionally log the error
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while saving the meal. {ex}");
            }
        }
    }
}
