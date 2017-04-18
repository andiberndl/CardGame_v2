using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CardGame_v2.Web.Models;
using CardGame_v2.DAL.EDM;
using CardGame_v2.DAL.Logic;

namespace CardGame_v2.Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserLogin user)
        {
            bool hasAccess = AuthManager.AuthUser(user.Email, user.Password);

            user.Role = UserManager.GetRoleByEmail(user.Email);

            //Hier findet die Überprüfung statt
            //...
            //Überprüfung Ende
            if (hasAccess)
            {
                var authTicket = new FormsAuthenticationTicket(
                    1,                              //Ticketversion
                    user.Email,                     //Useridentifizierung
                    DateTime.Now,                   //Zeitpunkt der Erstellung
                    DateTime.Now.AddMinutes(20),    //Gültigkeitsdauer
                    true,                           //Persistentes Ticket ueber Sessions hinweg
                    user.Role                       //Userrolle(n)
                    );

                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Error", "Error");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(User regUser)
        {
            var dbUser = new tblUser();

            dbUser.firstname = regUser.FirstName;
            dbUser.lastname = regUser.LastName;
            
            dbUser.userpassword = regUser.Password;
            dbUser.email = regUser.Email;

            dbUser.username = ""; //Im Moment nicht in Verwendung
            dbUser.currency = 1000; //Jeder faengt mit 1000 Waehrung an

            dbUser.fkUserRole = 2; //Jeder ist per Default ein Spieler

            if (AuthManager.Register(dbUser))
            {
                int userID = UserManager.GetUserByEmail(dbUser.email).idUser;
                if (DeckManager.AddDefaultDecksByUserId(userID))
                    return RedirectToAction("Login");
            }
            return RedirectToAction("Error", "Error");
        }
    }
}