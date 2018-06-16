using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TradeEngine.Entities;
using TradeEngine.Entities.LocalEntities;
using TradeEngine.UI.Auth_Data;

namespace TradeEngine.UI.Controllers
{
    [Auth]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {

            ViewBag.Date = DateTime.Today.ToString("yyyy-MM-dd");

            var vChartPoints = TERepository.Instance.GetVersionChartDetails();

            var pChartPoints = TERepository.Instance.GetProcessingChartDetails();

            if (vChartPoints == null || vChartPoints == null) return View();

            ViewBag.VersionXAxis = string.Join(",", vChartPoints.Keys.Select(x => AddDoubleQuotes(x)));

            ViewBag.VersionYAxis = string.Join(",", vChartPoints.Values);

            ViewBag.ProcessXAxis = string.Join(",", pChartPoints.Keys.Select(x => AddDoubleQuotes(x)));

            ViewBag.ProcessYAxis = string.Join(",", pChartPoints.Values);

            GetAVailableDates();

            return View();
        }



        [HttpGet]
        public ActionResult Extract()
        {
            //var path = Server.MapPath("~/App_Data/TradesXML.xml");
            //var xml = XDocument.Load(path);
            var list = new List<Trade>();

            //var trades = xml.Descendants("Trade");
            var random = new Random();
            for (int i = 0; i < random.Next(1, 12); i++)
            {
                var value = Math.Round(random.NextDouble() * 6, 2);
                var tradeId = $"T100{DateTime.Today.ToString("yyyyMMdd")}{random.Next(10, 20)}";

                if (!list.Where(x => x.TradeStatus == "NEW").Select(x => x.TradeId).Contains(tradeId))
                    list.Add(
                        new Trade
                        {
                            TradeId = tradeId,
                            TradeType = (random.Next(1, 22) * 100) % 2 == 0 ? "HEDGE" : "SPEC",
                            Quantity = (random.Next(1, 300) % 4 != 0) ? Convert.ToDouble(random.Next(20, 50)) + value : (Convert.ToDouble(random.Next(20, 50)) + value) - 100,
                            Price = Convert.ToDouble(random.Next(200, 350)) + value,
                            Instrument = "EEX",
                            BuySell = (value * 100) % 3 == 0 ? "BUY" : "SELL",
                            CreatedOn = DateTime.Now
                        }
                        );
            }

            TERepository.Instance.SaveTrades(list);

            Session["IsExtracted"] = true;

            GetAVailableDates();

            return View("Search", new TradesModel(list));
        }

        [HttpPost]
        public ActionResult Search(FormCollection collection)
        {
            var date = collection["selectedDate"];

            Session["Date"] = date;

            var list = TERepository.Instance.GetTrades(DateTime.Parse(date));

            if (list?.Count == 0)
            {

                return View("NotAvailable");
            }

            Session["IsExtracted"] = false;

            return View(new TradesModel(list.OrderBy(x => x.TradeId).ToList()));
        }

        private string AddDoubleQuotes(string value)
        {
            return "\"" + value + "\"";
        }

        private void GetAVailableDates()
        {
            var availableDates = TERepository.Instance.GetAvailableDates();

            var listDates = availableDates?.Select(x => new SelectListItem { Text = x, Value = x }).OrderBy(x => x.Text);

            if (listDates != null && listDates.Count() > 0)
                listDates.FirstOrDefault().Selected = true;
            else
                listDates = new List<SelectListItem> { new SelectListItem { Text = "No available extractions", Value = null, Selected = true, Disabled = true } }.OrderBy(x => x);

            Session["Dates"] = listDates;
        }
    }
}