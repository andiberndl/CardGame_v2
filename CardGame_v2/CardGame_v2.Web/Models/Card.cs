using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardGame_v2.Web.Models
{
    public class Card : IComparable<Card>
    {
        public int CardID { get; set; }
        public string CardName { get; set; }
        public string Type { get; set; }
        public int Mana { get; set; }
        public int Attack { get; set; }
        public int Life { get; set; }
        public byte[] Image { get; set; }

        public int CompareTo(Card other)
        {
            if (this.CardID == other.CardID)
                return 0;
            else if (this.CardID < other.CardID)
                return -1;
            else //this.CardID > other.CardID
                return 1;
        }
    }
}