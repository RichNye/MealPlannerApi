using MealPlannerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MealPlannerApi.Data;
using MealPlannerApi.Services;

namespace MealPlannerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private MealService _mealService;
        

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(Meal meal)
        {
            _context.Meals.Add(meal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = meal.Id }, meal);
        }
    }
}
