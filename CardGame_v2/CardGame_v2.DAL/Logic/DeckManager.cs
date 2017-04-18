using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CardGame_v2.DAL.EDM;
using CardGame_v2.Log;

namespace CardGame_v2.DAL.Logic
{
    public class DeckManager
    {
        public static tblDeck GetDeckById(int id)
        {
            tblDeck deck = null;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    deck = db.tblDeck.Find(id);
                    if (deck == null)
                    {
                        throw new Exception("DeckNotFound");
                    }
                    return deck;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return null;
            }
        }
        //Does not return the right number, and is not used anymore
        public static int GetNumDeckCardsById(int id)
        {
            tblDeck deck = null;
            int numCards = -1;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    deck = db.tblDeck.Find(id);
                    if (deck == null)
                    {
                        throw new Exception("DeckNotFound");
                    }
                    numCards = deck.tblDeckCard.Count;

                    return numCards;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return numCards;
            }
        }

        public static List<tblCard> GetDeckCardsById(int id)
        {
            var deckCards = new List<tblCard>();

            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    var dbDeck = db.tblDeck.Where(d => d.idDeck == id).FirstOrDefault();
                    if (dbDeck == null)
                    {
                        throw new Exception("UserDoesNotExist");
                    }
                    var dbDeckCollection = dbDeck.tblDeckCard.ToList();
                    if (dbDeckCollection == null)
                    {
                        throw new Exception("CardCollectionNotFound");
                    }
                    foreach (var dc in dbDeckCollection)
                    {
                        for (int i = 0; i < dc.numcards; i++)
                            deckCards.Add(dc.tblCard);
                    }

                    return deckCards;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return null;
            }
        }

        public static bool UpdateDeckById(int id, List<tblDeckCard> deckCards)
        {
            tblDeck deck = null;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    deck = db.tblDeck.Find(id);
                    if (deck == null)
                    {
                        throw new Exception("DeckNotFound");
                    }

                    var existingDeckList = deck.tblDeckCard.ToList();

                    foreach (var dc in existingDeckList)
                    {
                        deck.tblDeckCard.Remove(dc);
                    }
                    db.SaveChanges();

                    foreach (var dc in deckCards)
                    {
                        var dbDeckCard = new tblDeckCard();
                        dbDeckCard.numcards = dc.numcards;
                        dbDeckCard.tblDeck = db.tblDeck.Find(id);
                        dbDeckCard.tblCard = db.tblCard.Find(dc.tblCard.idCard);
                        db.tblDeckCard.Add(dbDeckCard);
                    }

                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return false;
            }
        }

        public static bool AddDeckByUserId(int id, string name)
        {
            tblUser user = null;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    user = db.tblUser.Find(id);
                    tblDeck deck = new tblDeck();
                    deck.deckname = name;
                    deck.tblUser = user;

                    db.tblDeck.Add(deck);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return false;
            }
        }

        public static bool AddDefaultDecksByUserId(int id)
        {
            tblUser user = null;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    user = db.tblUser.Find(id);
                    bool addedAll = false;
                    if (user == null)
                        throw new Exception("UserNotFound");

                    for (int i = 1; i <= 3; ++i)
                    {
                        addedAll = AddDeckByUserId(id, user.email.Substring(0,9) + i.ToString());
                    }
                    return addedAll;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return false;
            }
        }
    }
}