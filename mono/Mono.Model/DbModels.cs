using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{
    public class MyUser : IdentityUser
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        
        [Required]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        public Guid Token { get; set; }

        public DateTime? TokenDateTime { get; set; }

        public int? RestaurantID { get; set; }

        public virtual Restaurant Restaurant { get; set; }
    }

    public class Restaurant
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }      

        public string Description { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string OIB { get; set; }

        public virtual ICollection<MyUser> Users { get; set; }

        public virtual ICollection<Offer> Offers { get; set; }
    }

    public class CategorySize
    {
        public int ID { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public int Order { get; set; }
    }

    public class Category
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int SizeType { get; set; }

        [Display(Name = "Parent category")]
        //[ForeignKey("Category")]
        public int? ParentCategoryID { get; set; }

        [Display(Name = "Parent category")]
        public virtual Category ParentCategory { get; set; }

        public virtual ICollection<Food> Food { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }

        public virtual ICollection<Category> ChildCategory { get; set; }
    }

    public class Food
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Category")]
        public virtual Category Category { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        public int ID { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Display(Name = "Food")]
        public int? FoodID { get; set; }

        [Display(Name = "Category")]
        public int? CategoryID { get; set; }

        [Display(Name = "Food")]
        public virtual Food Food { get; set; }

        [Display(Name = "Category")]
        public virtual Category Category { get; set; }

        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
    }

    public enum Status
    {
        Active, Accepted, Expired
    }

    public class Order
    {
        public int ID { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        
        [Required]
        public Status Status { get; set; }

        public int? AcceptedOfferID { get; set; }
        
        [Required]
        public string UserID { get; set; }
        
        public virtual MyUser User { get; set; }

        [Display(Name="Items")]
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
        
        public virtual ICollection<Offer> Offers { get; set; }
    }

    public class FoodIngredient
    {
        public int ID { get; set; }

        public string Description { get; set; } //description for specific food

        [Required]
        public int FoodID { get; set; }

        public virtual Food Food { get; set; }

        public int? OrderID { get; set; }
        
        public virtual Order Order { get; set; }
               
        public virtual ICollection<Ingredient> Ingredients { get; set; }

        public int CategorySizeID { get; set; }

        [Display(Name="Size")]
        public virtual CategorySize CategorySize { get; set; }

        [Range(1, int.MaxValue)]
        public int Pieces { get; set; }

        [Required]
        public string UserID { get; set; }
    }

    public class Offer
    {
        public int ID { get; set; }

        public string Description { get; set; } //if they don't have something

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int DeliveryTime { get; set; } //minutes
        
        public int? AcceptedOrderID { get; set; }

        public DateTime? AcceptedDateTime { get; set; }
        
        [Required]
        public int RestaurantID { get; set; }
        
        public virtual Restaurant Restaurant { get; set; }
        
        [Required]
        public int OrderID { get; set; }
        
        public virtual Order Order { get; set; }
    }



}