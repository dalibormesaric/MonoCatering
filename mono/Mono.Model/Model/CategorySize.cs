using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{
   
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

}