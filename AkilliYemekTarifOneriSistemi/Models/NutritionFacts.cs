namespace AkilliYemekTarifOneriSistemi.Models
{
    public class NutritionFacts
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }


        
        public double Fiber { get; set; }
        public double Sugar { get; set; }
        public double Sodium { get; set; }
    }
}
