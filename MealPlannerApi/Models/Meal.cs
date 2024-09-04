using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealPlannerApi.Models
{
    public class Meal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime MealDate { get; set; }

        [Required]
        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }

        [Required]
        public string MealName { get; set; }

        public Recipe Recipe { get; set; }

        [ForeignKey("MealPlan")]
        public int? MealPlanId { get; set; }

        public MealPlan? MealPlan { get; set; }
    }
}
