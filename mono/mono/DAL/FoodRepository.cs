using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using mono.Models;

namespace mono.DAL
{
    public class FoodRepository : IFoodRepository, IDisposable
    {
        private readonly MonoDbContext _context;

        public FoodRepository(MonoDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public IEnumerable<Food> GetFoods()
        {
            //return _context.Foods.Include(i => i.Category).ToList();
            return _context.Foods.Include("Category").ToList();
        }

        public Food GetFoodByID(int id)
        {
            return _context.Foods.Find(id);
        }

        public IEnumerable<Food> GetFoodsByCategeoryID(int categoryID)
        {
            return _context.Foods.Include("Category").Where(f => f.CategoryID == categoryID).ToList();
        }

        public void InsertFood(Food food)
        {
            _context.Foods.Add(food);
        }

        public void DeleteFood(int foodID)
        {
            Food food = _context.Foods.Find(foodID);
            _context.Foods.Remove(food);
        }

        public void UpdateFood(Food food)
        {
            _context.Entry(food).State = System.Data.Entity.EntityState.Modified;
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