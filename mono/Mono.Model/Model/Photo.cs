using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mono.Model
{

    public class Photo
    {
        [Key]
        [Required]
        [RegularExpression("^[a-zA-Z0-9_.-]{3,15}$", ErrorMessage = "FileName is not valid")]
        public string FileName { get; set; } //if they don't have something
    }

}