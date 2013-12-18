using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using mono.Models;

namespace mono.DAL
{
    public class CategoryRepository : ICategoryRepository, IDisposable
    {
        private readonly MonoDbContext _context;

        public CategoryRepository(MonoDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public IEnumerable<Category> GetCategories()
        {
            //return _context.Categorys.Include(i => i.Category).ToList();
            return _context.Categories.Include("ParentCategory").ToList();
        }

        public Category GetCategoryByID(int id)
        {
            return _context.Categories.Find(id);
        }

        public void InsertCategory(Category category)
        {
            _context.Categories.Add(category);
        }

        public void DeleteCategory(int categoryID)
        {
            Category category = _context.Categories.Find(categoryID);
            _context.Categories.Remove(category);
        }

        public void UpdateCategory(Category category)
        {
            _context.Entry(category).State = System.Data.Entity.EntityState.Modified;
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