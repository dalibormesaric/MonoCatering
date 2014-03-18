using Mono.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Mono.Areas.Auction.Models
{
    public enum ListCategoryFoodItemType
    {
        Category, Food
    }

    public class ListCategoryFoodItem
    {
        public string Name { get; set; }
        public int ItemID { get; set; }
        public ListCategoryFoodItemType Type { get; set; }

    }

    public class ItemViewModel
    {
        public string Name { get; set; }
        public int? ParentCategoryID { get; set; }
        public List<ListCategoryFoodItem> ListCategoryFood { get; set; }
    }

    public class FoodIngredientViewModel
    {
        public int ID { get; set; }

        public string Description { get; set; }

        [Required]
        public int? FoodID { get; set; }

        public string FoodName { get; set; }

        [Display(Name = "Size")]
        public int CategorySizeID { get; set; } // name should be defined by category

        public string[] Ingredients { get; set; }

        [Range(1, int.MaxValue)]
        public int Pieces { get; set; }
    }
}