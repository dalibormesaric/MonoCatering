using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mono.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Mono.Data
{
    public class MonoDbContext : IdentityDbContext<MyUser>
    {
        public MonoDbContext()
            : base("DefaultConnection")
        {
        }      
        
        /*
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id).Property(p => p.Name).IsRequired();
            //modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
            //modelBuilder.Entity<IdentityUserLogin>().HasKey(u => new { u.UserId, u.LoginProvider, u.ProviderKey});
        }
        */
        
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<CategorySize> CategorySizes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Mono.Model.Order> Orders { get; set; }
        public DbSet<Mono.Model.FoodIngredient> FoodIngredients { get; set; }
        public DbSet<Mono.Model.Offer> Offers { get; set; }
        public DbSet<Mono.Model.Photo> Photos { get; set; }

        //public System.Data.Entity.DbSet<mono.Models.MyUser> IdentityUsers { get; set; }
    }
}