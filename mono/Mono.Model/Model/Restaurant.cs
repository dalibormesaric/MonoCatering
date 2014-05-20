using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{
   
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
    
}