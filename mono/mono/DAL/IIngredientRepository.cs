using System;
using System.Collections.Generic;
using mono.Models;

namespace mono.DAL
{
    public interface IIngredientRepository: IDisposable
    {
        IEnumerable<Ingredient> GetIngredients();
        Ingredient GetIngredientByID(int ingredientID);
        void InsertIngredient(Ingredient ingredient);
        void DeleteIngredient(int ingredientID);
        void UpdateIngredient(Ingredient ingredient);
        void Save();
    }
}