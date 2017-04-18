using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardGame_v2.Web.Models
{
    public class Order
    {
        public CardPack Pack { get; set; }
        public int Quantity { get; set; }
        public int UserBalance { get; set; }
    }
}