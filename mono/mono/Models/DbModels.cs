using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mono.Models
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
        
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

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

    public class Category
    {
        public int ID { get; set; }
       
        public string Name { get; set; }

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
    }

    public enum Status
    {
        Active, Accepted, Expired
    }

    public class Order
    {
        public int ID { get; set; }

        public int Description { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        
        [Required]
        public Status Status { get; set; }
        
        [Required]
        public int UserID { get; set; }
        
        public virtual MyUser User { get; set; }
        
        [Required]
        public virtual ICollection<OrderFood> FoodIngredients { get; set; }
        
        public virtual ICollection<Offer> Offers { get; set; }
    }

    public class OrderFood
    {
        public int ID { get; set; }

        public string Description { get; set; } //description for specific food

        [Required]
        public int OrderID { get; set; }
        
        public virtual Order Order { get; set; }
        
        public virtual ICollection<Food> Food { get; set; }
        
        public virtual ICollection<Ingredient> Ingredients { get; set; }

        public string Size { get; set; } // name should be defined by category

        public string Pieces { get; set; }
    }

    public class Offer
    {
        public int ID { get; set; }

        [Required]
        public decimal Price { get; set; }
        
        [Required]
        
        public DateTime DeliveryTime { get; set; }
        
        public string Description { get; set; } //if they don't have something

        [Required]
        public int RestaurantID { get; set; }
        
        public virtual Restaurant Restaurant { get; set; }
        
        [Required]
        public int OrderID { get; set; }
        
        public virtual Order Order { get; set; }
    }



}