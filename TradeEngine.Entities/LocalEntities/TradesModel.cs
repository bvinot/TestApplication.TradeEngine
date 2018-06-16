using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeEngine.Entities.LocalEntities
{
    public class TradesModel
    {

        public IEnumerable<Trade> Trades { get; set; }

        public IEnumerable<ProcessedTrade> ProcessedTrades { get; set; }

        public TradesModel()
        {

        }
        public TradesModel(List<Trade> trades)
        {
            Trades = trades;

        }
        public TradesModel(List<ProcessedTrade> pTrades)
        {
            ProcessedTrades = pTrades;
        }
    }
}
