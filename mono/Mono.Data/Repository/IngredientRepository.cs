using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class IngredientRepository : GenericRepository<Ingredient> 
    {
        public IngredientRepository()
            : base()
        {

        }
        public IngredientRepository(MonoDbContext context)
            : base(context)
        {

        }

        public virtual List<Ingredient> IngredientsForFood(Food food)
        {
            Category category = food.Category;

            IEnumerable<Ingredient> ingredients = category.Ingredients.Union(food.Ingredients);

            while (category.ParentCategoryID != null)
            {
                category = category.ParentCategory;
                ingredients = ingredients.Union(category.Ingredients);
            }

            return ingredients.OrderBy(i => i.Name).ToList();
        }

        public virtual List<Ingredient> IngredientsForFoodIngredient(List<int> ingredientsID)
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            foreach (var ingredientID in ingredientsID)
            {
                ingredients.Add(GetByID(ingredientID));
            }
            return ingredients.OrderBy(i => i.Name).ToList();
        }
    }
}