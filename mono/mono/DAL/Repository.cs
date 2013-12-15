using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using mono.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace mono.DAL
{
    public class MonoDbContext : IdentityDbContext
    {
        public MonoDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
    }

}