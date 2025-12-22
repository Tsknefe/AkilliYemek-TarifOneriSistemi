using System.ComponentModel.DataAnnotations;

namespace AkilliYemekTarifOneriSistemi.Models.ViewModels
{
    public class UserProfileEditViewModel
    {
        [Range(1, 120)]
        public int Age { get; set; }

        [Range(50, 260)]
        public double HeightCm { get; set; }

        [Range(20, 400)]
        public double WeightKg { get; set; }

        [Required]
        public string Gender { get; set; } = "Male";

        [Required]
        public string ActivityLevel { get; set; } = "sedentary";

        [Required]
        public string Goal { get; set; } = "Maintain";

        [Required]
        public string DietType { get; set; } = "Normal";
    }
}
