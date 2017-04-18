using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardGame_v2.Web.Models
{
    public class DeckBuilder
    {
        public int DeckID { get; set; }
        public List<Card> deck { get; set; }
        public List<Card> caco { get; set; }

        public DeckBuilder()
        {
            deck = new List<Card>();
            caco = new List<Card>();
        }
    }
}