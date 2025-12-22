using System.Text.Json.Serialization;

namespace AkilliYemekTarifOneriSistemi.Models
{
    public class NutritionApiResponse
    {
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; }

        [JsonPropertyName("nutriments")]
        public Nutriments Nutriments { get; set; }
    }

    public class Nutriments
    {
        [JsonPropertyName("energy-kcal_100g")]
        public double Energy { get; set; }

        [JsonPropertyName("proteins_100g")]
        public double Protein { get; set; }

        [JsonPropertyName("fat_100g")]
        public double Fat { get; set; }

        [JsonPropertyName("carbohydrates_100g")]
        public double Carbs { get; set; }

        [JsonPropertyName("sugars_100g")]
        public double Sugar { get; set; }

        [JsonPropertyName("fiber_100g")]
        public double Fiber { get; set; }

        [JsonPropertyName("sodium_100g")]
        public double Sodium { get; set; }
    }
}
