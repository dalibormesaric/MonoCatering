using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{
    public class Order
    {
        public int ID { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        
        [Required]
        public Status Status { get; set; }

        public int? AcceptedOfferID { get; set; }

        public DateTime? AcceptedDateTime { get; set; }
        
        [Required]
        public string UserID { get; set; }
        
        public virtual MyUser User { get; set; }

        [Display(Name="Items")]
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
        
        public virtual ICollection<Offer> Offers { get; set; }
    }
    
}