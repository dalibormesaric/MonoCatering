using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class FoodIngredientRepository : GenericRepository<FoodIngredient> 
    {
        public FoodIngredientRepository()
            : base()
        {

        }
        public FoodIngredientRepository(MonoDbContext context)
            : base(context)
        {

        }
    }
}