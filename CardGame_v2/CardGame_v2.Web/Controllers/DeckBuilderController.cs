using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardGame_v2.Web.Models;
using CardGame_v2.DAL.EDM;
using CardGame_v2.DAL.Logic;
using CardGame_v2.Log;

namespace CardGame_v2.Web.Controllers
{
    public class DeckBuilderController : Controller
    {
        // GET: DeckBuilder
        public ActionResult EditDeck(int id)
        {
            DeckBuilder db = new DeckBuilder();
            db.DeckID = id;
            var dbDeckCards = DeckManager.GetDeckCardsById(id);

            foreach (var cc in dbDeckCards)
            {
                Card card = new Card();
                card.CardID = cc.idCard;
                card.Attack = cc.attack;
                card.CardName = cc.cardname;
                card.Life = cc.life;
                card.Mana = cc.manacost;
                card.Image = cc.cardimage;
                card.Type = UserManager.CardTypeNames[cc.fkCardType ?? 0];
                card.Type = card.Type == "Minion" ? "M" : card.Type == "Spell" ? "S" : "W";

                db.deck.Add(card);
            }

            var dbCardList = UserManager.GetAllCardsByEmail(User.Identity.Name);

            foreach (var cc in dbCardList)
            {
                Card card = new Card();
                card.CardID = cc.idCard;
                card.Attack = cc.attack;
                card.CardName = cc.cardname;
                card.Image = cc.cardimage;
                card.Life = cc.life;
                card.Mana = cc.manacost;
                card.Type = UserManager.CardTypeNames[cc.fkCardType ?? 0];
                card.Type = card.Type == "Minion" ? "M" : card.Type == "Spell" ? "S" : "W";

                db.caco.Add(card);
            }

            foreach (var deckCard in db.deck)
            {
                int idx = db.caco.FindIndex(i => i.CardName == deckCard.CardName);
                db.caco.RemoveAt(idx);
            }

            db.caco.Sort();
            db.deck.Sort();

            TempData["DeckBuilder"] = db;
            return View(db);
        }

        public ActionResult AddCardToDeck(int id)
        {
            DeckBuilder db = new DeckBuilder();
            db = (DeckBuilder)TempData["DeckBuilder"];

            Card card = db.caco[id];
            db.caco.RemoveAt(id);

            db.deck.Add(card);

            db.caco.Sort();
            db.deck.Sort();

            TempData["DeckBuilder"] = db;
            return View("EditDeck", db);
        }

        public ActionResult RemoveCardFromDeck(int id)
        {
            DeckBuilder db = new DeckBuilder();
            db = (DeckBuilder)TempData["DeckBuilder"];

            Card card = db.deck[id];
            db.deck.RemoveAt(id);

            db.caco.Add(card);

            db.caco.Sort();
            db.deck.Sort();

            TempData["DeckBuilder"] = db;
            return View("EditDeck", db);
        }

        public ActionResult SaveDeck(int id)
        {
            DeckBuilder db = new DeckBuilder();
            db = (DeckBuilder)TempData["DeckBuilder"];

            var dbDeckList = new List<tblDeckCard>();

            foreach (var c in db.deck)
            {
                int idx = dbDeckList.FindIndex(i => i.tblCard.idCard == c.CardID);

                if (idx >= 0)
                {
                    dbDeckList[idx].numcards += 1;
                }
                else
                {
                    var dbCard = new tblCard();
                    dbCard.attack = c.Attack;
                    dbCard.cardname = c.CardName;
                    dbCard.cardimage = c.Image;
                    dbCard.idCard = c.CardID;
                    dbCard.life = c.Life;
                    dbCard.manacost = c.Mana;
                    //hier einfach nur die ID befuellen (muss DB id sein)
                    //im manager dann die ID raussuchen und die db hinzufuegen
                    var dbDeckCard = new tblDeckCard();
                    dbDeckCard.tblCard = dbCard;
                    dbDeckCard.numcards = 1;
                    //dbDeckCard.tblDeck = DeckManager.GetDeckById(id);

                    dbDeckList.Add(dbDeckCard);
                }
            }

            var result = DeckManager.UpdateDeckById(id, dbDeckList); //Macht irgendwas, aber irgendwas falsches!

            //Neue DeckCollection dem DeckManager geben

            return RedirectToAction("DeckOverview", "Profile");
        }
    }
}