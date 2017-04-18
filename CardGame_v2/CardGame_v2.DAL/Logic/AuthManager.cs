using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardGame_v2.DAL.EDM;
using CardGame_v2.Log;

namespace CardGame_v2.DAL.Logic
{
    public class AuthManager
    {
        public static bool Register(tblUser regUser)
        {
            try
            {
                using (var db = new CardGame_v2Entities())
                {
                    if (db.tblUser.Any(n => n.email == regUser.email))
                    {
                        throw new Exception("UserAlreadyExists");
                    }
                    //Salt erzeugen
                    string salt = Helper.GenerateSalt();

                    //Passwort Hashen
                    string hashedAndSaltedPassword = Helper.GenerateHash(regUser.userpassword + salt);

                    regUser.userpassword = hashedAndSaltedPassword;
                    regUser.usersalt = salt;

                    db.tblUser.Add(regUser);
                    db.SaveChanges();
                    Writer.LogInfo("AddedUser");
                }
            }
            catch (Exception e)
            {
                Writer.LogError(e);
            }
            return true;
        }

        public static bool AuthUser(string login, string password)
        {
            try
            {
                string dbUserPassword = null;
                string dbUserSalt = null;

                using (var db = new CardGame_v2Entities())
                {
                    tblUser dbUser = db.tblUser.Where(u => u.email == login).FirstOrDefault();
                    if (dbUser == null)
                    {
                        throw new Exception("UserDoesNotExist");
                    }

                    dbUserPassword = dbUser.userpassword;
                    dbUserSalt = dbUser.usersalt;

                    Writer.LogInfo("Entered Pass=" + password);

                    password = Helper.GenerateHash(password + dbUserSalt);

                    Writer.LogInfo("Hashed Pass=" + password);

                    if (dbUserPassword == password)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
