using System;
using System.Collections.Generic;
using mono.Models;

namespace mono.DAL
{
    public interface ICategoryRepository: IDisposable
    {
        IEnumerable<Category> GetCategories();
        Category GetCategoryByID(int categoryID);
        void InsertCategory(Category category);
        void DeleteCategory(int categoryID);
        void UpdateCategory(Category category);
        void Save();
    }
}