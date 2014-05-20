using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{
 
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

        [Display(Name = "Photo")]
        public string PhotoID { get; set; }

        public virtual ICollection<Food> Food { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }

        public virtual ICollection<Category> ChildCategory { get; set; }
    }

}