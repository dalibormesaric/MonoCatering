using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Mono.Areas.Admin.Models
{
    public class PhotoUploadViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_-]{3,15}$", ErrorMessage = "FileName is not valid, should be 3-15 charters long, and only a-z, A-Z, 0-9, _ or -.")]
        [Display(Name="File Name")]
        public string FileName { get; set; }

        [Required]
        public HttpPostedFileBase Image { get; set; }
    }

    public class PhotoEditViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_.-]{3,15}$", ErrorMessage = "FileName is not valid, should be 3-15 charters long, and only a-z, A-Z, 0-9, _ or -.")]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_.-]{3,15}$", ErrorMessage = "FileName is not valid, should be 3-15 charters long, and only a-z, A-Z, 0-9, _ or -.")]
        [Display(Name = "New File Name")]
        public string NewFileName { get; set; }
    }
}
