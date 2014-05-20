using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Model;

namespace Mono.Data
{
    public class UnitOfWork : IDisposable, Mono.Data.IUnitOfWork
    {
        private MonoDbContext context = new MonoDbContext();

        /*
        readonly Lazy<UserRepository> lazyUserRepository;
        readonly Lazy<RestaurantRepository> lazyRestaurantRepository;
        readonly Lazy<CategorySizeRepository> lazyCategorySizeRepository;
        readonly Lazy<CategoryRepository> lazyCategoryRepository;
        readonly Lazy<FoodRepository> lazyFoodRepository;
        readonly Lazy<IngredientRepository> lazyIngredientRepository;
        readonly Lazy<FoodIngredientRepository> lazyFoodIngredientRepository;
        readonly Lazy<OrderRepository> lazyOrderRepository;
        readonly Lazy<OfferRepository> lazyOfferRepository;
        readonly Lazy<PhotoRepository> lazyPhotoRepository;

        public UnitOfWork()
        {

        }

        public UnitOfWork(Lazy<UserRepository> userRepository,
            Lazy<RestaurantRepository> restaurantRepository,
            Lazy<CategorySizeRepository> categorySizeRepository,
            Lazy<CategoryRepository> categoryRepository,
            Lazy<FoodRepository> foodRepository,
            Lazy<IngredientRepository> ingredientRepository,
            Lazy<FoodIngredientRepository> foodIngredientRepository,
            Lazy<OrderRepository> orderRepository,
            Lazy<OfferRepository> offerRepository,
            Lazy<PhotoRepository> photoRepository)
        {
            this.lazyUserRepository = userRepository;
            this.lazyRestaurantRepository = restaurantRepository;
            this.lazyCategorySizeRepository = categorySizeRepository;
            this.lazyCategoryRepository = categoryRepository;
            this.lazyFoodRepository = foodRepository;
            this.lazyIngredientRepository = ingredientRepository;
            this.lazyFoodIngredientRepository = foodIngredientRepository;
            this.lazyOrderRepository = orderRepository;
            this.lazyOfferRepository = offerRepository;
            this.lazyPhotoRepository = photoRepository;
        }

        public virtual UserRepository UserRepository
        {
            get
            {
                return lazyUserRepository.Value;
            }
        }

        public virtual RestaurantRepository RestaurantRepository
        {
            get
            {
                return lazyRestaurantRepository.Value;
            }
        }

        public virtual CategorySizeRepository CategorySizeRepository
        {
            get
            {
                return lazyCategorySizeRepository.Value;
            }
        }

        public virtual CategoryRepository CategoryRepository
        {
            get
            {
                return lazyCategoryRepository.Value;
            }
        }

        public virtual FoodRepository FoodRepository
        {
            get
            {
                return lazyFoodRepository.Value;
            }
        }

        public virtual IngredientRepository IngredientRepository
        {
            get
            {
                return lazyIngredientRepository.Value;
            }
        }

        public virtual FoodIngredientRepository FoodIngredientRepository
        {
            get
            {
                return lazyFoodIngredientRepository.Value;
            }
        }

        public virtual OrderRepository OrderRepository
        {
            get
            {
                return lazyOrderRepository.Value;
            }
        }

        public virtual OfferRepository OfferRepository
        {
            get
            {
                return lazyOfferRepository.Value;
            }
        }

        public virtual PhotoRepository PhotoRepository
        {
            get
            {
                return lazyPhotoRepository.Value;
            }
        }
        */

        private UserRepository _userRepository;
        private RestaurantRepository _restaurantRepository;
        private CategorySizeRepository _categorySizeRepository;
        private CategoryRepository _categoryRepository;
        private FoodRepository _foodRepository;
        private IngredientRepository _ingredientRepository;
        private FoodIngredientRepository _foodIngredientRepository;
        private OrderRepository _orderRepository;
        private OfferRepository _offerRepository;
        private PhotoRepository _photoRepository;

        public virtual UserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(context);
                }
                return _userRepository;
            }
        }

        public virtual RestaurantRepository RestaurantRepository
        {
            get
            {
                if (_restaurantRepository == null)
                {
                    _restaurantRepository = new RestaurantRepository(context);
                }
                return _restaurantRepository;
            }
        }

        public virtual CategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(context);
                }
                return _categoryRepository;
            }
        }

        public virtual CategorySizeRepository CategorySizeRepository
        {
            get
            {
                if (_categorySizeRepository == null)
                {
                    _categorySizeRepository = new CategorySizeRepository(context);
                }
                return _categorySizeRepository;
            }
        }

        public virtual FoodRepository FoodRepository
        {
            get
            {
                if (_foodRepository == null)
                {
                    _foodRepository = new FoodRepository(context);
                }
                return _foodRepository;
            }
        }

        public virtual IngredientRepository IngredientRepository
        {
            get
            {
                if (_ingredientRepository == null)
                {
                    _ingredientRepository = new IngredientRepository(context);
                }
                return _ingredientRepository;
            }
        }

        public virtual FoodIngredientRepository FoodIngredientRepository
        {
            get
            {
                if (_foodIngredientRepository == null)
                {
                    _foodIngredientRepository = new FoodIngredientRepository(context);
                }
                return _foodIngredientRepository;
            }
        }

        public virtual OrderRepository OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new OrderRepository(context);
                }
                return _orderRepository;
            }
        }

        public virtual OfferRepository OfferRepository
        {
            get
            {
                if (_offerRepository == null)
                {
                    _offerRepository = new  OfferRepository(context);
                }
                return _offerRepository;
            }
        }

        public virtual PhotoRepository PhotoRepository
        {
            get
            {
                if (_photoRepository == null)
                {
                    _photoRepository = new PhotoRepository(context);
                }
                return _photoRepository;
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