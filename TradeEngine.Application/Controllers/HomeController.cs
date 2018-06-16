using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradeEngine.Entities;

namespace TradeEngine.Application.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            /*   var path = Server.MapPath("~/App_Data/TradesXML.xml");
                var xml = XDocument.Load(path);
                var list = new List<Trade>();

                var trades = xml.Descendants("Trade");
                var random = new Random();
                foreach (var item in trades)
                {
                    var value = Math.Round(random.NextDouble() * 6, 2);
                    list.Add(
                        new Trade
                        {
                            TradeId = item.Attribute("TradeId").Value,
                            TradeType = (value *100) % 2 == 0 ? item.Attribute("TradeType").Value : "SPEC",
                            Quantity = Convert.ToDouble(item.Attribute("Quantity").Value) + value,
                            Price = Convert.ToDouble(item.Attribute("Price").Value) + value,
                            Instrument = item.Attribute("Instrument").Value,
                            BuySell = (value * 100) % 3 == 0 ?item.Attribute("BuySell").Value: "SELL",
                            CreatedOn = !string.IsNullOrEmpty(item.Attribute("CreatedOn").Value) ? Convert.ToDateTime(item.Attribute("CreatedOn").Value) : DateTime.Today
                        }
                        );
                }

               TERepository.Instance.SaveTrades(list);*/



            return View();
        }

        [HttpPost]
        public ActionResult Search(DateTime date)
        {
            var list = TERepository.Instance.GetTrades(DateTime.Today);

            return View(list);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}