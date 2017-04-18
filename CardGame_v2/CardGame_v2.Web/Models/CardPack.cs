using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardGame_v2.Web.Models
{
    public class CardPack
    {
        public int CardPackID { get; set; }
        public string PackName { get; set; }
        public int NumCards { get; set; }
        public int Price { get; set; }
    }
}