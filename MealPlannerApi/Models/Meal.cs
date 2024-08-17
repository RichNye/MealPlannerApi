using System.ComponentModel.DataAnnotations;

namespace MealPlannerApi.Models
{
    public class Meal
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }
    }
}
