<<<<<<< HEAD
﻿using AkilliYemekTarifOneriSistemi.Models;
=======
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AkilliYemekTarifOneriSistemi.Models;
>>>>>>> emreaktas

namespace AkilliYemekTarifOneriSistemi.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetAllAsync();
        Task<Ingredient> GetByIdAsync(int id);
<<<<<<< HEAD
        Task<Ingredient> CreateAsync(Ingredient ingredient);
=======
        Task AddAsync(Ingredient ingredient);
        Task UpdateAsync(Ingredient ingredient);
        Task DeleteAsync(int id);
>>>>>>> emreaktas
    }
}
