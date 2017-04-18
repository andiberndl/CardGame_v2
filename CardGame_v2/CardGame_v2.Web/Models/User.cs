using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardGame_v2.Web.Models
{
    public class User : UserLogin
    {
        public int UserID { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Currency { get; set; }
    }
}