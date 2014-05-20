using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{

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

        [Display(Name = "Photo")]
        public string PhotoID { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }

}