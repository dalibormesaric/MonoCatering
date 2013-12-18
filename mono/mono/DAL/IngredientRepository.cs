using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using mono.Models;

namespace mono.DAL
{
    public class IngredientRepository : IIngredientRepository, IDisposable
    {
        private readonly MonoDbContext _context;

        public IngredientRepository(MonoDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public IEnumerable<Ingredient> GetIngredients()
        {
            //return _context.Ingredients.Include(i => i.Category).Include(i => i.Food).ToList();
            return _context.Ingredients.Include("Category").Include("Food").ToList();
        }

        public Ingredient GetIngredientByID(int id)
        {
            return _context.Ingredients.Find(id);
        }

        public void InsertIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
        }

        public void DeleteIngredient(int ingredientID)
        {
            Ingredient ingredient = _context.Ingredients.Find(ingredientID);
            _context.Ingredients.Remove(ingredient);
        }

        public void UpdateIngredient(Ingredient ingredient)
        {
            _context.Entry(ingredient).State = System.Data.Entity.EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}