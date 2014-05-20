using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{
   
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

}