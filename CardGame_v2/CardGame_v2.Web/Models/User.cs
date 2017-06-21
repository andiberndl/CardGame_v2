using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CardGame_v2.Web.Models
{
    public class User : UserLogin
    {
        public int UserID { get; set; }
        public string Salt { get; set; }

        [Required(ErrorMessage = "required!", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z--]+$", ErrorMessage = "only upper and lowercase letters allowed")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "required!", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z--]+$", ErrorMessage = "only upper and lowercase letters allowed")]
        public string LastName { get; set; }

        public string Street { get; set; }

        public int ZipCode { get; set; }

        public string City { get; set; }

        public decimal Currency { get; set; }

        public DateTime RegDate { get; set; }
    }
}