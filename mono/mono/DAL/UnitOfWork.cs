using System;
using System.Collections.Generic;
using System.Linq;
using mono.Models;

namespace mono.DAL
{
    public class UnitOfWork : IDisposable
    {
        private MonoDbContext context = new MonoDbContext();

        private GenericRepository<MyUser> userRepository;
        private GenericRepository<Restaurant> restaurantRepository;
        private GenericRepository<Category> categoryRepository;
        private GenericRepository<Food> foodRepository;
        private GenericRepository<Ingredient> ingredientRepository;

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
    }
}