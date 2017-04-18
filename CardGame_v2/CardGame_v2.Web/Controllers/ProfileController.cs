using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardGame_v2.Web.Models;
using CardGame_v2.DAL.EDM;
using CardGame_v2.DAL.Logic;

namespace CardGame_v2.Web.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        [HttpGet]
        [Authorize(Roles = "player,admin")]
        public ActionResult Index()
        {
            UserProfile profile = new UserProfile();

            var dbUser = UserManager.GetUserByEmail(User.Identity.Name);

            profile.Currency = dbUser.currency;
            profile.Email = dbUser.email;
            profile.FirstName = dbUser.firstname;
            profile.LastName = dbUser.lastname;

            profile.NumDistinctCardsOwned = UserManager.GetNumDistinctCardsOwnedByEmail(User.Identity.Name);
            profile.NumDecksOwned = UserManager.GetNumDecksOwnedByEmail(User.Identity.Name);

            profile.NumTotalCardsOwned = UserManager.GetNumTotalCardsOwnedByEmail(User.Identity.Name);

            return View(profile);
        }

        [HttpGet]
        [Authorize(Roles = "player,admin")]
        public ActionResult CardCollection()
        {
            var cardCollection = new List<Card>();

            var dbCardList = UserManager.GetAllCardsByEmail(User.Identity.Name);

            foreach (var cc in dbCardList)
            {
                Card card = new Card();
                card.CardID = cc.idCard;
                card.Attack = cc.attack;
                card.CardName = cc.cardname;
                card.Life = cc.life;
                card.Mana = cc.manacost;
                card.Type = UserManager.CardTypeNames[cc.fkCardType ?? 0];

                cardCollection.Add(card);
            }

            return View(cardCollection);
        }

        [HttpGet]
        [Authorize(Roles = "player,admin")]
        public ActionResult DeckOverview()
        {
            var decks = new List<Deck>();

            var dbDecks = UserManager.GetAllDecksByEmail(User.Identity.Name);

            foreach(var d in dbDecks)
            {
                Deck deck = new Deck();
                deck.DeckID = d.idDeck;
                deck.Name = d.deckname;
                decks.Add(deck);
            }

            return View(decks);
        }

        [HttpGet]
        [Authorize(Roles = "player,admin")]
        public ActionResult DeckDetails(int id)
        {
            var deckCards = new List<Card>();

            var dbDeckCards = DeckManager.GetDeckCardsById(id);

            foreach (var cc in dbDeckCards)
            {
                Card card = new Card();
                card.CardID = cc.idCard;
                card.Attack = cc.attack;
                card.CardName = cc.cardname;
                card.Life = cc.life;
                card.Mana = cc.manacost;
                card.Type = UserManager.CardTypeNames[cc.fkCardType ?? 0];

                deckCards.Add(card);
            }

            return View(deckCards);
        }
    }
}