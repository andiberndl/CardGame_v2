using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardGame_v2.DAL.EDM;
using CardGame_v2.Log;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;

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

                    var rubypack = (from r in db.tblCardPack
                                    where r.idCardPack == packID
                                    select r).FirstOrDefault();

                    int orderID = (from p in db.tblVirtualPurchase
                                   orderby p.idVirtualPurchase descending
                                   select p.idVirtualPurchase).FirstOrDefault();

                    foreach (var value in updatePerson)
                    {
                        value.currency += (int)goldValue;
                    }
                    db.SaveChanges();

                    var updatePersonvar = (from p in db.tblUser
                                           where p.idUser == personID
                                           select p).FirstOrDefault();

                    var pack = (from q in db.tblCardPack
                                where q.idCardPack == packID
                                select q).FirstOrDefault();

                    SmtpClient client = new SmtpClient("srv08.itccn.loc");
                    client.Credentials = new NetworkCredential("andreas.berndl-forstner@qualifizierung.at", "BBRZforstner1992");
                    client.Port = 25;
                    client.EnableSsl = false;

                    MailMessage mess = new MailMessage();
                    mess.IsBodyHtml = true;


                    mess.From = new MailAddress("andreas.berndl-forstner@qualifizierung.at");
                    mess.To.Add(new MailAddress($"{updatePersonvar.email}"));
                    //mess.To.Add(new MailAddress("martin.hengsberger@qualifizierung.at"));


                    mess.Subject = "purchase confirmation!";
                    mess.Body = $"<p style='font-size:20px'>Thank you for your purchase! </br >" +
                                $"<p><b>bill number:</b> {orderID}</p>" +
                                $"<p><b>paid:</b> {pack.packprice} € (including 20% ​​tax)</p>" +
                                $"<p><b>date of purchase:</b> {order.timeofpurchase}</p> </br >" +
                                $"<p><b>purchased package:</b> {pack.packname}</p>" +
                                $"<p><b>goldquantity:</b> {goldValue}</p> </br>" +
                                $"<p>We wish you a lot of fun!</p>" +
                                "<p><b>MTP-Group</b><br/ > Simmeringer Hauptstrasse XX<br/>1030 Wien<br/> UID:78946513</p>";


                    //Create billing pdf for Email Attachment 

                    Document document = new Document(PageSize.A4, 50, 30, 20, 20);


                    //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("C:/Users/hengmart/Documents/GitHub/LAP_Project/CardGame/CardGame.Web/img/CS_Logo.png");
                    //logo.ScalePercent(40);
                    //logo.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                    MemoryStream stream = new MemoryStream();
                    PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                    pdfWriter.CloseStream = false;

                    //START PDF CREATION
                    document.Open();

                    //Add Information 
                    //document.Add(logo);
                    document.Add(new Paragraph($"\nThank you for your purchase!\n\n"));
                    document.Add(new Paragraph($"Firstname: {updatePersonvar.firstname}\nLastname: {updatePersonvar.lastname}\n\n"));

                    Paragraph headline = new Paragraph("Purchase\n\n");
                    headline.Font.Size = 32;

                    document.Add(headline);

                    document.Add(new Paragraph($"Billingnumber: {orderID}"));
                    document.Add(new Paragraph($"Paid: {pack.packprice} € (including 20% ​​tax)"));
                    document.Add(new Paragraph($"Date of Purchase: {order.timeofpurchase}"));
                    document.Add(new Paragraph($"Purchased pack: {pack.packname}"));
                    document.Add(new Paragraph($"Goldquantity: {goldValue} Rubys"));

                    document.Add(new Paragraph($"\nWe wish you a lot of fun!\n\n"));

                    document.Add(new Paragraph("MTP-Group\nSimmeringer Hauptstrasse XX\n1030 Wien\nUID:78946513"));



                    //Text für Rechnung

                    //END PDF CREATION


                    document.Close();

                    stream.Flush(); //Always catches me out
                    stream.Position = 0; //Not sure if this is required


                    //////////////////////////////////////////////////////////////
                    mess.Attachments.Add(new Attachment(stream, "billing.pdf"));


                    client.Send(mess);


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
