namespace AkilliYemekTarifOneriSistemi.Services.DietRules
{
    public static class DietTypeNormalizer
    {
        public static string Normalize(string? diet)
        {
            if (string.IsNullOrWhiteSpace(diet)) return "normal";

            var d = diet.Trim().ToLowerInvariant();

            if (d == "vegan") return "vegan";
            if (d == "vejetaryen" || d == "vejeteryan") return "vejetaryen";
            if (d == "glutensiz") return "glutensiz";
            if (d == "keto") return "keto";

            if (d == "vegetarian") return "vejetaryen";
            if (d == "glutenfree" || d == "gluten-free" || d == "gluten free") return "glutensiz";
            if (d == "ketogenic") return "keto";

            if (d == "normal" || d == "standart" || d == "standard" || d == "none" || d == "omnivore")
                return "normal";

            return "normal";
        }

        public static bool IsRestricted(string normalizedDiet)
        {
            var d = Normalize(normalizedDiet);
            return d != "normal";
        }
    }
}
