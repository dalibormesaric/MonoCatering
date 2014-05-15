using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mono.Data;

namespace Mono.Models
{
    public class HomeViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string PhotoID { get; set; }

        public int Count { get; set; }
    }

}
