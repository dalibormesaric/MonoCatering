using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class FoodRepository : GenericRepository<Food> 
    {
        public FoodRepository()
            : base()
        {

        }
        public FoodRepository(MonoDbContext context)
            : base(context)
        {

        }

        public virtual IEnumerable<Food> FoodInCategory(Category category)
        {
            var foods = category.Food.AsEnumerable();

            Stack<Category> childs = new Stack<Category>();
            childs.Push(category);

            do
            {
                category = childs.Pop();

                foods = foods.Union(category.Food);

                foreach (var child in category.ChildCategory)
                    childs.Push(child);

            } while (childs.Count != 0);

            return foods;
        }
    }
}