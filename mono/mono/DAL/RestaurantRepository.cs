using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using mono.Models;

namespace mono.DAL
{
    public class RestaurantRepository : IRestaurantRepository, IDisposable
    {
        private readonly MonoDbContext _context;

        public RestaurantRepository(MonoDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public IEnumerable<Restaurant> GetRestaurants()
        {
            return _context.Restaurants.ToList();
        }

        public Restaurant GetRestaurantByID(int id)
        {
            return _context.Restaurants.Find(id);
        }

        public void InsertRestaurant(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
        }

        public void DeleteRestaurant(int restaurantID)
        {
            Restaurant restaurant = _context.Restaurants.Find(restaurantID);
            _context.Restaurants.Remove(restaurant);
        }

        public void UpdateRestaurant(Restaurant restaurant)
        {
            _context.Entry(restaurant).State = System.Data.Entity.EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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