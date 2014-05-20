using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository()
            : base()
        {

        }
        public CategoryRepository(MonoDbContext context)
            : base(context)
        {

        }

        public virtual IEnumerable<Category> SubCategories(Category category)
        {
            var categories = category.ChildCategory.AsEnumerable();

            Stack<Category> childs = new Stack<Category>();
            childs.Push(category);

            do
            {
                category = childs.Pop();

                foreach (var child in category.ChildCategory)
                {
                    categories = categories.Union(child.ChildCategory);
                    childs.Push(child);
                }

            } while (childs.Count != 0);

            return categories;
        }
    }
}