using MealPlannerApi.Data;
using MealPlannerApi.Models;
using MealPlannerApi.Models.DTOs.MealPlan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/mealplans")]
    public class MealPlanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MealPlanController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _context.MealPlans
                .Include(mp => mp.Meals).ThenInclude(m => m.Recipe)
                .OrderByDescending(mp => mp.Id)
                .ToListAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var mealplan = await _context.MealPlans
                .Include(mp => mp.Meals).ThenInclude(m => m.Recipe)
                .FirstOrDefaultAsync(mp => mp.Id == id);

            if (mealplan == null)
            {
                return NotFound($"No meal plan with {id} found.");
            }

            return Ok(mealplan);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] List<int> Meals)
        {
            List<int> failedIds = new List<int>();
            List<int> successfulIds = new List<int>();
            MealPlan mealPlan = new MealPlan();
            foreach(int id in Meals)
            {
                Meal meal = await _context.Meals.FindAsync(id);
                if(meal == null)
                {
                    // no meal found with that Id.
                    failedIds.Add(id);
                }
                else
                {
                    mealPlan.Meals.Add(meal);
                    meal.MealPlanId = mealPlan.Id;
                    successfulIds.Add(id);
                }
            }

            if(mealPlan.Meals.Count > 0)
            {
                try
                {
                    _context.MealPlans.Add(mealPlan);
                    await _context.SaveChangesAsync();

                    MealPlanResponseDTO response = new MealPlanResponseDTO()
                    {
                        MealPlanId = mealPlan.Id,
                        MealIds = successfulIds
                    };

                    return Ok(response);
                }
                catch
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound("No meals found with the given ids.");
            }
        }

        // delete the meal plan, as well as any meals associated with it.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _context.MealPlans.AnyAsync(mp => mp.Id == id);
            if (!exists)
                return NotFound($"No meal plan found with id of {id}");

            // Single DELETE statement: DELETE FROM "Meals" WHERE "MealPlanId" = @id
            await _context.Meals
                .Where(m => m.MealPlanId == id)
                .ExecuteDeleteAsync();

            // Single DELETE statement: DELETE FROM "MealPlans" WHERE "Id" = @id
            await _context.MealPlans
                .Where(mp => mp.Id == id)
                .ExecuteDeleteAsync();

            return Ok("meal plan deleted");
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] MealPlanDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            MealPlan mealPlan = await _context.MealPlans.FindAsync(id);

            if (mealPlan == null)
            {
                return NotFound($"No meal plan for with id of {id}");
            }
            else
            {
                mealPlan.Meals = dto.Meals;
                try
                {
                    int result = await _context.SaveChangesAsync();
                    return Ok($"{result} row(s) updated successfully.");
                }
                catch
                {
                    return BadRequest();
                } 
            }
        }
    }
}
