using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mono.Models
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            this.DateTime = System.DateTime.Now;
        }

        public string ID { get; set; }
        [Display(Name = "Date time")]
        public DateTime DateTime { get; set; }
        [Display(Name = "Restaurant")]
        public string Restaurant { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "User")]
        public string UserName { get; set; }
    }

    public class OfferViewModel
    {
        public OfferViewModel()
        {

        }

        public string ID { get; set; }
        [Display(Name = "Category")]
        public String Category { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Size")]
        public string Size { get; set; }
        [Display(Name = "Pieces")]
        public string Pieces { get; set; }
    }
    

}
