using System.ComponentModel.DataAnnotations;

namespace MealPlannerApi.Models.DTOs.MealPlan
{
    public class MealPlanDTO
    {
        [Required]
        public ICollection<Meal> Meals { get; set; }
    }
}
