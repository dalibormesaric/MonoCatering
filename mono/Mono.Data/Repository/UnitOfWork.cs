using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Model;

namespace Mono.Data
{
    public class UnitOfWork : IDisposable
    {
        private MonoDbContext context = new MonoDbContext();

        private GenericRepository<MyUser> userRepository;
        private GenericRepository<Restaurant> restaurantRepository;
        private GenericRepository<CategorySize> categorySizeRepository;
        private GenericRepository<Category> categoryRepository;
        private GenericRepository<Food> foodRepository;
        private GenericRepository<Ingredient> ingredientRepository;

        private GenericRepository<FoodIngredient> foodIngredientRepository;
        private GenericRepository<Order> orderRepository;
        private GenericRepository<Offer> offerRepository;

        public virtual GenericRepository<MyUser> UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new GenericRepository<MyUser>(context);
                }
                return userRepository;
            }
        }

        public virtual GenericRepository<Restaurant> RestaurantRepository
        {
            get
            {
                if (restaurantRepository == null)
                {
                    restaurantRepository = new GenericRepository<Restaurant>(context);
                }
                return restaurantRepository;
            }
        }

        public virtual GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (categoryRepository == null)
                {
                    categoryRepository = new GenericRepository<Category>(context);
                }
                return categoryRepository;
            }
        }

        public virtual GenericRepository<CategorySize> CategorySizeRepository
        {
            get
            {
                if (categorySizeRepository == null)
                {
                    categorySizeRepository = new GenericRepository<CategorySize>(context);
                }
                return categorySizeRepository;
            }
        }

        public virtual GenericRepository<Food> FoodRepository
        {
            get
            {
                if (foodRepository == null)
                {
                    foodRepository = new GenericRepository<Food>(context);
                }
                return foodRepository;
            }
        }

        public virtual GenericRepository<Ingredient> IngredientRepository
        {
            get
            {
                if (ingredientRepository == null)
                {
                    ingredientRepository = new GenericRepository<Ingredient>(context);
                }
                return ingredientRepository;
            }
        }

        public virtual GenericRepository<FoodIngredient> FoodIngredientRepository
        {
            get
            {
                if (foodIngredientRepository == null)
                {
                    foodIngredientRepository = new GenericRepository<FoodIngredient>(context);
                }
                return foodIngredientRepository;
            }
        }

        public virtual GenericRepository<Order> OrderRepository
        {
            get
            {
                if (orderRepository == null)
                {
                    orderRepository = new GenericRepository<Order>(context);
                }
                return orderRepository;
            }
        }

        public virtual GenericRepository<Offer> OfferRepository
        {
            get
            {
                if (offerRepository == null)
                {
                    offerRepository = new GenericRepository<Offer>(context);
                }
                return offerRepository;
            }
        }

        public virtual void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region HelperFunctions

        public IEnumerable<CategorySize> SizeValues(int sizeType)
        {
            return CategorySizeRepository.Get(s => s.Type == sizeType, q => q.OrderBy(s => s.Order), "");
        }

        public virtual String SizeValuesString(int sizeType)
        {
            return string.Join(", ", SizeValues(sizeType).Select(s => s.Value).ToArray());
        }

        private List<int> SizeTypes()
        {
            return CategorySizeRepository.Get().Select(s => s.Type).OrderBy(s => s).Distinct().ToList();
        }

        public class TypeSelectList
        {
            public int ID { get; set; }
            public string SizeValuesString { get; set; }
        }

        public virtual List<TypeSelectList> SizeValuesSelectList()
        {
            List<TypeSelectList> selectList = new List<TypeSelectList>();

            var sizeTypes = SizeTypes();
            foreach (var sizeType in sizeTypes)
            {
                selectList.Add(new TypeSelectList { ID = sizeType, SizeValuesString = SizeValuesString(sizeType) });
            }

            return selectList;
        }

        public virtual List<Ingredient> IngredientsForFood(Food food)
        {
            Category category = CategoryRepository.GetByID(food.CategoryID);

            IEnumerable<Ingredient> ingredients = category.Ingredients.Union(food.Ingredients);

            while (category.ParentCategoryID != null)
            {
                category = CategoryRepository.GetByID((int)category.ParentCategoryID);
                ingredients = ingredients.Union(category.Ingredients);
            }

            return ingredients.OrderBy(i => i.Name).ToList();
        }

        public virtual List<Ingredient> IngredientsForFoodIngredient(List<int> ingredientsID)
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            foreach (var ingredientID in ingredientsID)
            {
                ingredients.Add(IngredientRepository.GetByID(ingredientID));
            }
            return ingredients.OrderBy(i => i.Name).ToList();
        }

        #endregion
    }
}