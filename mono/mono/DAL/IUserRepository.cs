using System;
using System.Collections.Generic;
using mono.Models;

namespace mono.DAL
{
    public interface IUserRepository: IDisposable
    {
        IEnumerable<User> GetUsers();
        IEnumerable<User> GetUsersByRestaurantID(int RestaurantID);
        User GetUserByID(string UserID);
        void InsertUser(User User);
        void DeleteUser(int UserID);
        void UpdateUser(User User);
        void Save();
    }
}