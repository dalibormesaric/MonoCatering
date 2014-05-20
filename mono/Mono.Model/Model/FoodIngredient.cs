using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{

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

}