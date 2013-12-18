using System;
using System.Collections.Generic;
using System.Linq;
using mono.Models;

namespace mono.DAL
{
    public class UnitOfWork : IDisposable
    {
        private MonoDbContext context = new MonoDbContext();

        private GenericRepository<User> userRepository;
        private GenericRepository<Restaurant> restaurantRepository;
        private GenericRepository<Category> categoryRepository;
        private GenericRepository<Food> foodRepository;
        private GenericRepository<Ingredient> ingredientRepository;

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new GenericRepository<User>(context);
                }
                return userRepository;
            }
        }

        public GenericRepository<Restaurant> RestaurantRepository
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

        public GenericRepository<Category> CategoryRepository
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

        public GenericRepository<Food> FoodRepository
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

        public GenericRepository<Ingredient> IngredientRepository
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

        public void Save()
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