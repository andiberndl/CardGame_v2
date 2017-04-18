using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardGame_v2.Web.Models
{
    public class Deck
    {
        public int DeckID { get; set; }
        public string Name { get; set; }
        public List<Card> Cards { get; set; }
    }
}