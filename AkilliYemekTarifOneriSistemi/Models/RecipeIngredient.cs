namespace AkilliYemekTarifOneriSistemi.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        public double Quantity { get; set; }            //ondalıklı miktarlarımız için 
        public string Unit { get; set; }                //adet,kg,g gibi nicelikler için 


    }
}
