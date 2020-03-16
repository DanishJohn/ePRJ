using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ePRJ.Models
{
    public class Account
    {
        [Required]
        [MaxLength(200,ErrorMessage = "Please enter maximum length of 200")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "E-Mail")]
        public string UserEmail { get; set; }
        [Required]
        [MaxLength(200,ErrorMessage ="Maximum length of 200")]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }
        [MaxLength(100,ErrorMessage = "Maximum length of 100")]
        [Display(Name="Full Name")]
        public string UserFullName { get; set; }
        [MaxLength(100,ErrorMessage = "Maximum Length of 100")]
        [Display(Name = "Address")]
        public string UserAddress { get; set; }
        [Required]
        [Display(Name="Status")]
        public bool UserStatus { get; set; }
        [Required]
        [Display(Name="Role")]
        public string UserRole { get; set; }
    }
}