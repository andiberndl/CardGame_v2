using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CardGame_v2.Web.Models
{
    public class UserLogin
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "required!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "invalide mailadress")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [StringLength(maximumLength: 20, MinimumLength = 4, ErrorMessage = "password requirements not met (4-20 digits)")]
        [Required(ErrorMessage = "required!", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "required!", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "passwords not identical!")]
        [DisplayName("confirmPassword")]
        public string PasswordValidation { get; set; }


        public string Role { get; set; }
    }
}