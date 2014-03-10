﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using mono.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace mono.DAL
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
        
        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<CategorySize> CategorySizes { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Food> Foods { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }

        public System.Data.Entity.DbSet<mono.Models.Order> Orders { get; set; }

        public System.Data.Entity.DbSet<mono.Models.FoodIngredient> FoodIngredients { get; set; }

        public System.Data.Entity.DbSet<mono.Models.Offer> Offers { get; set; }

        //public System.Data.Entity.DbSet<mono.Models.MyUser> IdentityUsers { get; set; }
    }
}