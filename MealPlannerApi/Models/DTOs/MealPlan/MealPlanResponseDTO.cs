namespace MealPlannerApi.Models.DTOs.MealPlan
{
    public class MealPlanResponseDTO
    {
        public int MealPlanId { get; set; }

        public List<int> MealIds { get; set; }
    }
}
