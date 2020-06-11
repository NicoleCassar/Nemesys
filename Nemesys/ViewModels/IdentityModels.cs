using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Nemesys.Models;

namespace Nemesys.ViewModels
{
        public class LoginViewModel
        {
            [Required]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public class RegisterViewModel
        {
        
            [Required]
            [Display(Name = "ID Number")]
            public string IdNum { get; set; }
            
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [Required]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Please confirm your password")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Confirm password doesn't match. Please type it in carefully.")]
            public string ConfirmPassword { get; set; }
        }
    }
