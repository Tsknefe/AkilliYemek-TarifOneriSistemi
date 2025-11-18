using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AkilliYemekTarifOneriSistemi.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        //Data Annotians kısımları validations kısımları için 
        [Required(ErrorMessage="Tarif Adının Girilmesi Zorunludur")]
        [StringLength(100,ErrorMessage="Tarif Adı En Fazla 100 Karakter Olmalıdır...")]
        public string Name { get; set; }

        [Required(ErrorMessage="Açıklama Yapılması Zorunludur...")]
        [StringLength(500,ErrorMessage="Açıklama En Fazla 500 Karakter İçermelidir...")]
        public string Description { get; set; }

        [Range(1,300,ErrorMessage="Pişirme Süresi 1-300 Dakika Arasında Olmalıdır...")]
        public int CookingTime { get; set; }

        [Range(1,20,ErrorMessage= "Servis Sayısı 1-20 Arasında olmalıdır...")]
        public int Servings { get; set; }

        [Required(ErrorMessage="Diyet Tipi Boş Olamaz...")]
        public string DietType { get; set; }      



        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public NutritionFacts NutritionFacts { get; set; }
    }
}
