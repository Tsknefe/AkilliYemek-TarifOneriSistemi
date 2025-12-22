namespace AkilliYemekTarifOneriSistemi.Models
{
    public class UserAllergy
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;
    }
}
