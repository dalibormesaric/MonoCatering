using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mono.Models
{
    public class AdminUserViewModel
    {
        [Required]
        public string ID { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Restaurant")]
        public string Restaurant { get; set; }
    }
}
