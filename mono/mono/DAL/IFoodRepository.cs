using System;
using System.Collections.Generic;
using mono.Models;

namespace mono.DAL
{
    public interface IFoodRepository: IDisposable
    {
        IEnumerable<Food> GetFoods();
        Food GetFoodByID(int foodID);
        IEnumerable<Food> GetFoodsByCategeoryID(int categoryID);
        void InsertFood(Food food);
        void DeleteFood(int foodID);
        void UpdateFood(Food food);
        void Save();
    }
}