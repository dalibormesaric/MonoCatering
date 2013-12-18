using System;
using System.Collections.Generic;
using mono.Models;

namespace mono.DAL
{
    public interface IRestaurantRepository: IDisposable
    {
        IEnumerable<Restaurant> GetRestaurants();
        Restaurant GetRestaurantByID(int restaurantID);
        void InsertRestaurant(Restaurant restaurant);
        void DeleteRestaurant(int restaurantID);
        void UpdateRestaurant(Restaurant restaurant);
        void Save();
    }
}