using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using mono.Models;

namespace mono.DAL
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly MonoDbContext _context;

        public UserRepository(MonoDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.Include("restaurant").ToList();
        }

        public IEnumerable<User> GetUsersByRestaurantID(int restaurantID)
        {
            return _context.Users.Where(u => u.RestaurantID == restaurantID);
        }

        public User GetUserByID(string id)
        {
            return _context.Users.Find(id);
        }

        public void InsertUser(User user)
        {
            _context.Users.Add(user);
        }

        public void DeleteUser(int userID)
        {
            User user = _context.Users.Find(userID);
            _context.Users.Remove(user);
        }

        public void UpdateUser(User user)
        {
            _context.Entry(user).State = System.Data.Entity.EntityState.Modified;
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