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

        public static int GetTotalCost(int id, int numPacks)
        {
            int price = 0;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    var pack = db.tblCardPack.Find(id);
                    if(pack == null)
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
            return price * numPacks;
        }

        public static List<tblCard> Order(int id, int numPacks)
        {
            //3 Steps: Generate Cards, Enter into DB
            Random rng = new Random();
            var generatedCards = new List<tblCard>();

            //Generate Cards
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    var cardPack = db.tblCardPack.Find(id);

                    if (cardPack == null)
                    {
                        throw new Exception("CardPackNotFound");
                    }
                    int numCardsToGenerate = cardPack.numcards;

                    numCardsToGenerate *= numPacks;

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
    }
}
