using CardGame_v2.DAL.EDM;
using CardGame_v2.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace CardGame_v2.Web.Controllers
{
    public class ChartController : Controller
    {
        // GET: Chart
        public ActionResult Index()
        {
            return View();
        }

        //Tutorial
        //https://www.aspsnippets.com/Articles/Google-Charts-in-ASPNet-MVC-Google-Pie-Doughnut-Chart-example-with-database-in-ASPNet-MVC.aspx
        [HttpPost]
       public JsonResult Chart()
        {
            //Create select statment
            string query = "select top 3 count(packname) as amount, packname ";
            query += "from tblVirtualPurchase join tblCardPack on idCardPack = fkCardPack group by packname order by amount desc";
            //Declare Conectionstring
            string constr = "Data Source=localhost;" + "Initial Catalog=CardGame_v2;" + "User id=sa;" + "Password=123user!";
            List<object> chardata = new List<object>();
            chardata.Add(new object[]
            {
                "packname", "amount"
            });

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            chardata.Add(new object[]
                            {
                                sdr["packname"], sdr["amount"]
                            });
                        }
                    }

                    con.Close();
                }
            }

            return Json(chardata);
        }


    }
}