using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{

    public class Offer
    {
        public int ID { get; set; }

        public string Description { get; set; } //if they don't have something

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [RegularExpression(@"[0-9]*,?[0-9]{0,2}", ErrorMessage = "Invalid format for {0}.")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int DeliveryTime { get; set; } //minutes
               
        [Required]
        public int RestaurantID { get; set; }
        
        public virtual Restaurant Restaurant { get; set; }
        
        [Required]
        public int OrderID { get; set; }
        
        public virtual Order Order { get; set; }
    }

}