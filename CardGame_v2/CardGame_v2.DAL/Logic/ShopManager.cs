using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardGame_v2.DAL.EDM;
using CardGame_v2.Log;
using System.Data.Entity;

namespace CardGame_v2.DAL.Logic
{
    public class ShopManager
    {
        public static List<tblCardPack> GetAllCardPacks()
        {
            var allCardPacks = new List<tblCardPack>();

            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    allCardPacks = db.tblCardPack.ToList();
                }
                if (allCardPacks == null)
                    throw new Exception("NoCardPacksFound");
            }
            catch (Exception e)
            {
                Writer.LogError(e);
            }
            return allCardPacks;
        }

        public static List<tblCardPack> GetRubyPacks()
        {
            List<tblCardPack> rubyPacks = new List<tblCardPack>();

            using (var db = new CardGame_v2Entities())
            {
                rubyPacks = db.tblCardPack.Where(rP => rP.numcards == 0).ToList();
            }

            return rubyPacks;
        }

        public static List<tblCardPack> GetCardPacks()
        {
            List<tblCardPack> cardPacks = new List<tblCardPack>();

            using (var db = new CardGame_v2Entities())
            {
                cardPacks = db.tblCardPack.Where(cP => cP.numcards != 0).ToList();
            }

            return cardPacks;
        }


        public static tblCardPack GetCardPackById(int id)
        {
            var dbCardPack = new tblCardPack();

            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    dbCardPack = db.tblCardPack.Find(id);
                }
                if (dbCardPack == null)
                    throw new Exception("CardPackNotFound");
            }
            catch (Exception e)
            {
                Writer.LogError(e);
            }
            return dbCardPack;
        }

        public static int GetTotalCost(int id)
        {
            int price = 0;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    var pack = db.tblCardPack.Find(id);
                    if (pack == null)
                    {
                        throw new Exception("PackNotFound");
                    }
                    price = pack.packprice;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
            }
            return price;
        }

        public static List<tblCard> Order(int idPack, int idUser)
        {
            //3 Steps: Generate Cards, Enter into DB
            tblVirtualPurchase order = new tblVirtualPurchase();
            Random rng = new Random();
            var generatedCards = new List<tblCard>();

            using (var db = new CardGame_v2Entities())
            {
                order.fkCardPack = idPack;
                order.fkUser = idUser;
                order.timeofpurchase = DateTime.Now;
                db.tblVirtualPurchase.Add(order);
                db.SaveChanges();
            }

           
            

            //Generate Cards
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    var cardPack = db.tblCardPack.Find(idPack);

                    if (cardPack == null)
                    {
                        throw new Exception("CardPackNotFound");
                    }
                    int numCardsToGenerate = cardPack.numcards;



                    var validIDs = db.tblCard.Select(c => c.idCard).ToList();

                    Writer.LogInfo("vID: " + validIDs.Count.ToString());

                    if (validIDs.Count == 0)
                    {
                        throw new Exception("NoCardIDsFound");
                    }

                    for (int i = 0; i < numCardsToGenerate; ++i)
                    {
                        int indexID = rng.Next(0, validIDs.Count - 1);
                        int generatedCardID = validIDs[indexID];
                        var generatedCard = db.tblCard.Where(c => c.idCard == generatedCardID).Include(c => c.tblCardType).FirstOrDefault();

                        if (generatedCard == null)
                        {
                            throw new Exception("CardNotFound");
                        }
                        generatedCards.Add(generatedCard);
                    }
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
            }

            foreach (var c in generatedCards)
            {
                Writer.LogInfo("Card: " + c.idCard);
            }

            //Cards were successfully generated
            //Now we can enter them into the DB



            return generatedCards;
        }

        /// <summary>
        /// for Gold Packs
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="packID"></param>
        /// <param name="creditCardNumber"></param>
        public static void ExecuteOrder(int personID, int packID, string creditCardNumber)
        {
            using (var db = new CardGame_v2Entities())
            {
                tblVirtualPurchase order = new tblVirtualPurchase();
                //tblUserCardCollection col = new tblUserCardCollection();
                //Random r = new Random();

                order.fkCardPack = packID;
                order.fkUser = personID;
                order.timeofpurchase = DateTime.Now;
                db.tblVirtualPurchase.Add(order);
                db.SaveChanges();



                #region Goldpacks


                if (true)
                {
                    tblUser person = new tblUser();
                    var updatePerson = (from p in db.tblUser
                                        where p.idUser == personID
                                        select p);

                    var goldValue = (from g in db.tblCardPack
                                     where g.idCardPack == packID
                                     select g.RubyAmount).FirstOrDefault();

                    foreach (var value in updatePerson)
                    {
                        value.currency += (int)goldValue;
                    }
                    db.SaveChanges();
                }
                else
                {
                    //was auch immer 
                }

            }
            #endregion
        }
    }
}
