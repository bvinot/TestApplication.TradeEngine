using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TradeEngine.Entities.DBEntities;

namespace TradeEngine.Entities
{
    public sealed class TERepository
    {
        TradeEngineDataBaseContext context;
        private static readonly Lazy<TERepository> lazy = new Lazy<TERepository>(() => new TERepository());

        public static TERepository Instance { get { return lazy.Value; } }

        private TERepository()
        {
            context = new TradeEngineDataBaseContext();
        }

        public Dictionary<string, int> GetVersionChartDetails()
        {
            try
            {
                var results = context.TradesList.Select(x => new { x.CreatedOn, x.Version }).GroupBy(
                                                 p => DbFunctions.TruncateTime(p.CreatedOn),
                                                 p => p.Version, (key, g) => new { Date = key, VersionCount = g.Distinct().Count() }).ToDictionary(x => x.Date.Value.ToShortDateString(), x => x.VersionCount);

                return results;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public Dictionary<string, int> GetProcessingChartDetails()
        {
            try
            {
                var results = context.ProcessedTrades.Select(x => new { x.CreatedOn, x.Version }).GroupBy(
                                                 p => DbFunctions.TruncateTime(p.CreatedOn),
                                                 p => p.Version, (key, g) => new { Date = key, VersionCount = g.Distinct().Count() }).ToDictionary(x => x.Date.Value.ToShortDateString(), x => x.VersionCount);

                return results;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public void SaveTrades(List<Trade> trades)
        {

            var availableDates = context.TradesList?.Select(x => DbFunctions.TruncateTime(x.CreatedOn))?.Distinct();

            var date = trades.First().CreatedOn.Date;

            var lastVersion = availableDates != null && availableDates.Contains(trades.First().CreatedOn.Date) ?
                              context.TradesList.Where(x => DbFunctions.TruncateTime(x.CreatedOn) == date).Max(x => x.Version) : 0;

            trades.ForEach(x => { x.Version = lastVersion + 1; x.IsActive = true; });

            MarkTradeStatus(trades);

            context.TradesList.AddRange(trades);

            context.SaveChanges();
        }

        private void MarkTradeStatus(List<Trade> trades)
        {
            foreach (var trade in trades)
            {
                trade.TradeStatus = context.TradesList.Any(x => x.TradeId == trade.TradeId) ? "AMEND" : "NEW";
            }
        }

        public List<Trade> GetTrades(DateTime date)
        {
            var list = new List<Trade>();

            var dbTrades = context.TradesList.Where(x => DbFunctions.TruncateTime(x.CreatedOn) == date && x.IsActive && !x.IsProcessed);

            foreach (var trade in dbTrades)
            {
                list.Add(new Trade
                {
                    BuySell = trade.BuySell,
                    TradeId = trade.TradeId,
                    Instrument = trade.Instrument,
                    Price = trade.Price,
                    Quantity = trade.Quantity,
                    TradeStatus = trade.TradeStatus,
                    TradeType = trade.TradeType,
                    CreatedOn = trade.CreatedOn,
                    Version = trade.Version
                });
            }

            return list;
        }

        public IEnumerable<Trade> AppplyFilters(DateTime date)
        {
            var dbTrades = context.TradesList.Where(x => DbFunctions.TruncateTime(x.CreatedOn) == date && x.IsActive);

            var amendTradeIds = dbTrades.Where(x => x.TradeStatus == "AMEND").Select(x => x.TradeId).Distinct();

            var negationTrades = dbTrades.ToList().Where(x => x.Quantity < 0 && x.TradeStatus == "NEW" && !amendTradeIds.Contains(x.TradeId)).ToList();

            negationTrades.ForEach(x => { x.IsActive = false; x.IsProcessed = true; });

            context.OrphanTrades.AddRange(negationTrades.Select(x => new OrphanTrade(x)));

            foreach (var trade in context.OrphanTrades)
            {
                var oTrade = context.TradesList.SingleOrDefault(x => x.InputId == trade.InputId);

                context.TradesList.Remove(oTrade);
            }

            context.SaveChanges();

            return negationTrades;
        }

        public IEnumerable<ProcessedTrade> ApplyPricing(DateTime date)
        {
            var dbTrades = context.ProcessedTrades.Where(x => DbFunctions.TruncateTime(x.CreatedOn) == date).ToList();

            var groupedTrades = dbTrades.GroupBy(x => x.TradeId);//.Where(x => x.Count() > 1);

            var pricedTrades = groupedTrades.ToDictionary(x => x.Key, x => new Tuple<int, double>(x.Max(y => y.InputId), Math.Round(x.Select(y => y.Price).Average())));

            var finalTrades = dbTrades.Where(x => pricedTrades.ContainsKey(x.TradeId)).Select(x => UpdatePrice(x, pricedTrades)).ToList();

            var updatedTradeIds = pricedTrades.Values.Select(x => x.Item1);

            foreach (var trade in context.ProcessedTrades)
            {
                trade.TradeStatus = trade.Quantity <= 0 ? "IGNORED" : "PRICED";
                trade.IsIgnored = trade.Quantity <= 0;
            }

            var removableTrades = dbTrades.Where(x => pricedTrades.ContainsKey(x.TradeId) && !updatedTradeIds.Contains(x.InputId));

            foreach (var trade in removableTrades)
            {
                context.ProcessedTrades.Remove(trade);
            }

            foreach (var trade in context.TradesList)
            {
                trade.IsProcessed = finalTrades.Any(x => x.TradeId == trade.TradeId) && trade.Quantity > 0;
            }

            context.SaveChanges();
            
            return context.ProcessedTrades.Where(x => DbFunctions.TruncateTime(x.CreatedOn) == date);
        }

        public IEnumerable<ProcessedTrade> AggregateTrades(DateTime date)
        {
            var trades = context.TradesList.ToList().Where(x => x.CreatedOn.Date == date && x.IsActive && !x.IsProcessed).ToList();

            var tradeIds = trades.Select(x => x.TradeId).Distinct();

            var negatedTrades = context.OrphanTrades.Where(x => tradeIds.Contains(x.TradeId) && x.TradeStatus == "NEW");

            if (negatedTrades.Any())
            {
                context.TradesList.AddRange(negatedTrades.Select(x => new Trade(x)));
                trades.AddRange(negatedTrades.Select(x => new Trade(x)));

                foreach (var nTrade in negatedTrades) { context.OrphanTrades.Remove(nTrade); }
            }

            var localTrades = trades.Select(x => new ProcessedTrade(x, 0, 0));

            var groupedTrades = localTrades.GroupBy(x => x.TradeId).Where(x => x.Count() > 1);

            var aggregatedTrades = groupedTrades.ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

            var finalTrades = trades.Select(x => UpdateQuantity(x, aggregatedTrades)).ToList();

            var availableDates = context.ProcessedTrades?.Select(x => DbFunctions.TruncateTime(x.CreatedOn))?.Distinct();

            var cDate = trades.FirstOrDefault()?.CreatedOn.Date;

            var lastVersion = availableDates != null && availableDates.Contains(trades.First().CreatedOn.Date) ?
                              context.TradesList.Where(x => DbFunctions.TruncateTime(x.CreatedOn) == cDate).Max(x => x.Version) : 0;

            var processedTrades = finalTrades;//.ToList();

            processedTrades.ForEach(x => { x.TradeStatus = x.TradeStatus == "AMEND" ? "AGGREGATED" : x.TradeStatus; x.Version = lastVersion + 1; x.IsProcessed = true; });

            finalTrades.ForEach(x => { x.IsProcessed = true; });

            context.ProcessedTrades.AddRange(processedTrades);

            context.SaveChanges();

            return processedTrades;
        }

        public IEnumerable<string> GetAvailableDates()
        {
            return context.TradesList.ToList().Select(x => x.CreatedOn.ToShortDateString()).Distinct();
        }

        private IEnumerable<Trade> Filter(IQueryable<Trade> dbTrades)
        {
            var negationTrades = dbTrades.ToList().Where(x => x.Quantity < 0 && x.TradeStatus == "NEW").ToList();

            negationTrades.ForEach(x => { x.IsActive = false; x.IsProcessed = true; });

            context.OrphanTrades.AddRange(negationTrades.Select(x => new OrphanTrade(x)));

            context.SaveChanges();

            return negationTrades;
        }




        private ProcessedTrade UpdateQuantity(Trade trade, Dictionary<string, double> dict)
        {
            return dict.ContainsKey(trade.TradeId) ? new ProcessedTrade(trade, dict[trade.TradeId], 0) : new ProcessedTrade(trade, 0, 0);
        }


        private Trade UpdatePrice(ProcessedTrade trade, Dictionary<string, Tuple<int, double>> dict)
        {
            if (dict.ContainsKey(trade.TradeId))
            {
                trade.NewPrice = dict[trade.TradeId].Item2;
            }

            return new Trade(trade);
        }
    }
}
