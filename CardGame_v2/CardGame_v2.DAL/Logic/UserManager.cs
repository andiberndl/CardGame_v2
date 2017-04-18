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
    public class UserManager
    {
        public static readonly Dictionary<int, string> UserRoleNames;

        public static readonly Dictionary<int, string> CardTypeNames;

        static UserManager()
        {
            UserRoleNames = new Dictionary<int, string>();
            CardTypeNames = new Dictionary<int, string>();

            List<tblUserRole> userRoleList = null;
            List<tblCardType> cardTypeList = null;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    userRoleList = db.tblUserRole.ToList();
                    cardTypeList = db.tblCardType.ToList();
                }
                foreach (var role in userRoleList)
                {
                    UserRoleNames.Add(role.idUserrole, role.rolename);

                }

                foreach (var type in cardTypeList)
                {
                    CardTypeNames.Add(type.idCardType, type.typename);
                }

                UserRoleNames.Add(0, "n/a");
                CardTypeNames.Add(0, "n/a");
            }
            catch (Exception e)
            {
                Writer.LogError(e);
            }
        }

        public static List<tblUser> GetAllUsers()
        {
            List<tblUser> userList = null;
            using (var db = new CardGame_v2Entities())
            {
                userList = db.tblUser.ToList();
            }

            return userList;
        }

        public static tblUser GetUserByEmail(string email)
        {
            tblUser dbUser = null;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    dbUser = db.tblUser.Where(u => u.email == email).FirstOrDefault();
                    if (dbUser == null)
                    {
                        throw new Exception("UserDoesNotExist");
                    }
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
            }
            return dbUser;
        }

        public static string GetRoleByEmail(string email)
        {
            string role = "";
            using (var db = new CardGame_v2Entities())
            {
                tblUser dbUser = db.tblUser.Where(u => u.email == email).FirstOrDefault();
                if (dbUser == null)
                {
                    throw new Exception("UserDoesNotExist");
                }
                role = dbUser.tblUserRole.rolename;
            }
            return role;
        }

        public static int GetNumDistinctCardsOwnedByEmail(string email)
        {
            int numCards = -1;
            using (var db = new CardGame_v2Entities())
            {
                tblUser dbUser = db.tblUser.Where(u => u.email == email).FirstOrDefault();
                if (dbUser == null)
                {
                    throw new Exception("UserDoesNotExist");
                }
                numCards = dbUser.tblUserCardCollection.Count;
            }
            return numCards;
        }

        public static int GetNumTotalCardsOwnedByEmail(string email)
        {
            int numCards = -1;
            using (var db = new CardGame_v2Entities())
            {
                tblUser dbUser = db.tblUser.Where(u => u.email == email).FirstOrDefault();
                if (dbUser == null)
                {
                    throw new Exception("UserDoesNotExist");
                }
                numCards = 0;
                foreach (var c in dbUser.tblUserCardCollection)
                {
                    numCards += c.numcards;
                }

            }
            return numCards;
        }

        public static int GetNumDecksOwnedByEmail(string email)
        {
            int numDecks = -1;
            using (var db = new CardGame_v2Entities())
            {
                tblUser dbUser = db.tblUser.Where(u => u.email == email).FirstOrDefault();
                if (dbUser == null)
                {
                    throw new Exception("UserDoesNotExist");
                }
                numDecks = dbUser.tblDeck.Count;
            }
            return numDecks;
        }

        public static int GetBalanceByEmail(string email)
        {
            return GetUserByEmail(email).currency;
        }

        public static List<tblCard> GetAllCardsByEmail(string email)
        {
            var cardList = new List<tblCard>();

            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    var dbUser = db.tblUser.Where(u => u.email == email).FirstOrDefault();
                    if (dbUser == null)
                    {
                        throw new Exception("UserDoesNotExist");
                    }
                    var dbCardCollection = dbUser.tblUserCardCollection.ToList();
                    if (dbCardCollection == null)
                    {
                        throw new Exception("CardCollectionNotFound");
                    }
                    foreach (var cc in dbCardCollection)
                    {
                        for (int i = 0; i < cc.numcards; i++)
                            cardList.Add(cc.tblCard);
                    }
                    return cardList;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return null;
            }
        }

        public static bool UpdateBalanceByEmail(string email, int newBalance)
        {
            var dbUser = GetUserByEmail(email);

            dbUser.currency = newBalance;
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    db.Entry(dbUser).State = EntityState.Modified;
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

        public static bool AddCardsToCollectionByEmail(string email, List<tblCard> cards)
        {
            var dbUser = new tblUser();
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    dbUser = db.tblUser.Where(u => u.email == email).FirstOrDefault();
                    if (dbUser == null)
                    {
                        throw new Exception("UserDoesNotExist");
                    }

                    foreach (var c in cards)
                    {
                        var userCC = (from coll in db.tblUserCardCollection
                                      where coll.fkCard == c.idCard && coll.fkUser == dbUser.idUser
                                      select coll)
                                     .FirstOrDefault();

                        if (userCC == null) //User does not own card, add to collection
                        {

                            var cc = new tblUserCardCollection();
                            cc.tblCard = db.tblCard.Find(c.idCard);
                            cc.tblUser = dbUser;
                            cc.numcards = 1;
                            dbUser.tblUserCardCollection.Add(cc);
                            db.SaveChanges();
                        }
                        else //User owns card, add to num
                        {
                            userCC.numcards += 1;
                            db.Entry(userCC).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return false;
            }
        }

        public static List<tblDeck> GetAllDecksByEmail(string email)
        {
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    var dbUser = db.tblUser.Where(u => u.email == email).FirstOrDefault();
                    if (dbUser == null)
                    {
                        throw new Exception("UserDoesNotExist");
                    }
                    var dbDecks = dbUser.tblDeck.ToList();
                    if (dbDecks == null)
                    {
                        throw new Exception("NoDecksFound");
                    }

                    return dbDecks;
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
                return null;
            }
        }
    }
}
