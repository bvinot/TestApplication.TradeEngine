using System;
using System.Linq;
using System.Web.Mvc;
using TradeEngine.Entities;
using TradeEngine.Entities.LocalEntities;
using TradeEngine.UI.Auth_Data;

namespace TradeEngine.UI.Controllers
{
    public class ProcessController : Controller
    {
        [LocalCache(Duration =15)]
        public ActionResult ApplyFilters()
        {
            var selectedDate = DateTime.Parse(Session["Date"]?.ToString());

            var trades = TERepository.Instance.AppplyFilters(selectedDate);

            Session["CurrentProcess"] = "FilterApplied";

            var model = new TradesModel(trades.OrderBy(x => x.TradeId).ToList());

            return View("Process", model);
        }

        public ActionResult AgrregateQuantity()
        {
            var selectedDate = DateTime.Parse(Session["Date"]?.ToString());

            var trades = TERepository.Instance.AggregateTrades(selectedDate);

            Session["CurrentProcess"] = "AggregateApplied";

            var model = new TradesModel(trades.OrderBy(x => x.TradeId).ToList());

            return View("Process", model);
        }

        public ActionResult ApplyPricing()
        {
            var selectedDate = DateTime.Parse(Session["Date"]?.ToString());

            var trades = TERepository.Instance.ApplyPricing(selectedDate);

            Session["CurrentProcess"] = "PricingApplied";

            var model = new TradesModel(trades.OrderBy(x => x.TradeId).ToList());

            return View("Process", model);
        }
    }
}