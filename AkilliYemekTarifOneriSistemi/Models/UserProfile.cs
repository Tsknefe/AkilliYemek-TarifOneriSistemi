using Microsoft.AspNetCore.Identity;

namespace AkilliYemekTarifOneriSistemi.Models
{
    
    
    public class UserProfile
    {
        
        public int Id { get; set; }

        
        public string UserId { get; set; } = null!;

        
        public IdentityUser User { get; set; } = null!;

        
        public int Age { get; set; }
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }

        
        public string Gender { get; set; } = "Male";

        
        
        public string ActivityLevel { get; set; } = "sedentary";

        
        
        public string Goal { get; set; } = "Maintain";

        
        
        public string DietType { get; set; } = "Normal";
    }
}
