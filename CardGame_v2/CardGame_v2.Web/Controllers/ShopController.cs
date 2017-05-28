using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardGame_v2.Web.Models;
using CardGame_v2.DAL.EDM;
using CardGame_v2.DAL.Logic;
using CardGame_v2.Log;
using WindowsApplication1;

namespace CardGame_v2.Web.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        [HttpGet]
        [Authorize(Roles = "player")]
        public ActionResult Index()
        {
            Shop shop = new Shop();
            shop.cardPacks = new List<CardPack>();
            shop.rubyPacks = new List<RubyPack>();

            var dbCardPacks = ShopManager.GetCardPacks();

            foreach (var dbCp in dbCardPacks)
            {
                CardPack cardPack = new CardPack();
                cardPack.CardPackID = dbCp.idCardPack;
                cardPack.PackName = dbCp.packname;
                cardPack.NumCards = dbCp.numcards;
                cardPack.Price = dbCp.packprice;
                shop.cardPacks.Add(cardPack);
            }

            var dbRubyPacks = ShopManager.GetRubyPacks();

            foreach (var dbRp in dbRubyPacks)
            {
                RubyPack rubyPack = new RubyPack();
                rubyPack.CardPackID = dbRp.idCardPack;
                rubyPack.PackName = dbRp.packname;
                rubyPack.Price = dbRp.packprice;
                rubyPack.RubyAmount = (int)dbRp.RubyAmount;
                shop.rubyPacks.Add(rubyPack);

            }

            return View(shop);
        }

        [HttpPost]
        [Authorize(Roles = "player,admin")]
        public ActionResult Index(int packID, string creditCardNumber = "")
        {
            if (CreditCardVerification.IsValidCardNumber(creditCardNumber))
            {
                int userID = UserManager.GetUserByEmail(User.Identity.Name).idUser;
                var pid = packID;
                ShopManager.ExecuteOrder(userID, pid, creditCardNumber);

                TempData["orderComplete"] = "purchase complete!";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["orderAbort"] = "Creditcard data wrong!";
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        [Authorize(Roles = "player")]
        public ActionResult BuyCardPack(int id)
        {
            var dbCardPack = ShopManager.GetCardPackById(id);

            CardPack cardPack = new CardPack();
            cardPack.CardPackID = dbCardPack.idCardPack;
            cardPack.PackName = dbCardPack.packname;
            cardPack.NumCards = dbCardPack.numcards;
            cardPack.Price = dbCardPack.packprice;

            return View(cardPack);
        }

        [HttpPost]
        [Authorize(Roles = "player")]
        public ActionResult BuyCard(int idPack, int idUser) //,int numPacks)
        {
            Writer.LogInfo("id: " + idPack.ToString());
            //Writer.LogInfo("numPacks: " + numPacks.ToString());

            Order o = new Order();
            var dbCardPack = ShopManager.GetCardPackById(idPack);

            CardPack cardPack = new CardPack();
            cardPack.CardPackID = dbCardPack.idCardPack;
            cardPack.PackName = dbCardPack.packname;
            cardPack.NumCards = dbCardPack.numcards;
            cardPack.Price = dbCardPack.packprice;

            o.Pack = cardPack;
            o.IdUser = idUser;
            //o.Quantity = numPacks;
            o.UserBalance = UserManager.GetBalanceByEmail(User.Identity.Name);

            TempData["Order"] = o;

            return RedirectToAction("OrderOverview");
        }

        [HttpGet]
        [Authorize(Roles = "player")]
        public ActionResult OrderOverview()
        {
            Order o = (Order)TempData["Order"];
            TempData["Order"] = o;
            return View(o);
        }

        [HttpPost]
        [Authorize(Roles = "player")]
        [ActionName("OrderOverview")]
        public ActionResult Order()
        {
            Order o = (Order)TempData["Order"];
            //Check if User has enough balance
            try
            {
                var orderTotal = ShopManager.GetTotalCost(o.Pack.CardPackID);
                if (orderTotal > o.UserBalance)
                {
                    return RedirectToAction("NotEnoughBalance");
                }



                //User has enough money, subtract money
                var newBalance = o.UserBalance - orderTotal;

                var hasUpdated = UserManager.UpdateBalanceByEmail(User.Identity.Name, newBalance);
                if (!hasUpdated)
                {
                    return RedirectToAction("BalanceUpdateError");
                }

                //Generate Cards
                var orderedCards = ShopManager.Order(o.Pack.CardPackID, CardGame_v2.DAL.Logic.UserManager.GetUserByEmail(User.Identity.Name).idUser);

                //Add Cards to User Collection
                var hasUpdatedCards = UserManager.AddCardsToCollectionByEmail(User.Identity.Name, orderedCards);

                //evtl extra spalte in cardcollection mit fk fuer den Order machen

                TempData["OrderedCards"] = orderedCards;
                return RedirectToAction("ShowGeneratedCards");
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return RedirectToAction("Error", "Error");
            }

        }

        [HttpGet]
        [Authorize(Roles = "player")]
        public ActionResult ShowGeneratedCards()
        {
            var orderedCards = (List<tblCard>)TempData["OrderedCards"];
            var cards = new List<Card>();

            foreach (var c in orderedCards)
            {
                Card card = new Card();
                card.CardID = c.idCard;
                card.CardName = c.cardname;
                card.Type = c.tblCardType.typename;
                card.Mana = c.manacost;
                card.Image = c.cardimage;
                card.Attack = c.attack;
                card.Life = c.life;
                cards.Add(card);
            }
            //mit tempdata Orderd cards hier ansehen
            //bzw. nach aenderung einfach nur die Order ID abrufen und dann nach den jeweiligen karten suchen
            return View(cards);
        }

        [HttpGet]
        [Authorize(Roles = "player")]
        public ActionResult NotEnoughBalance()
        {
            return View();
        }

    }
}
