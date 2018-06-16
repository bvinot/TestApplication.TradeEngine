using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeEngine.Entities
{
    public class TradeBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column(Order = 0)]
        public int InputId { get; set; }

        [Column(Order = 1)]
        public string TradeId { get; set; }

        [Column(Order = 2)]
        public string TradeType { get; set; }

        [Column(Order = 3)]
        public string Instrument { get; set; }

        [Column(Order = 4)]
        public double Quantity { get; set; }

        [Column(Order = 5)]
        public double Price { get; set; }

        [Column(Order = 9)]
        public string BuySell { get; set; }

        [Column(Order = 10)]
        public DateTime CreatedOn { get; set; }

        [Column(Order = 11)]
        public string TradeStatus { get; set; }

        [Column(Order = 12)]
        public bool IsActive { get; set; }

        [Column(Order = 13)]
        public bool IsProcessed { get; set; }

        [Column(Order = 15)]
        public int Version { get; set; }

    }

    public class Trade : TradeBase
    {


        public Trade()
        {

        }
        public Trade(ProcessedTrade trade)
        {
            InputId = trade.InputId;

            TradeId = trade.TradeId;

            TradeType = trade.TradeType;

            Instrument = trade.Instrument;

            Quantity = trade.Quantity;

            Price = trade.Price;

            BuySell = trade.BuySell;

            CreatedOn = DateTime.Now;

            TradeStatus = trade.TradeStatus;

            IsActive = trade.IsActive;

            IsProcessed = trade.IsProcessed;

            Version = trade.Version;
        }
        public Trade(OrphanTrade trade)
        {
            InputId = trade.InputId;

            TradeId = trade.TradeId;

            TradeType = trade.TradeType;

            Instrument = trade.Instrument;

            Quantity = trade.Quantity;

            Price = trade.Price;

            BuySell = trade.BuySell;

            CreatedOn = DateTime.Now;

            TradeStatus = trade.TradeStatus;

            IsActive = trade.IsActive;

            IsProcessed = trade.IsProcessed;

            Version = trade.Version;
        }
    }

    public class OrphanTrade : TradeBase
    {
        public OrphanTrade()
        {

        }

        public OrphanTrade(Trade trade)
        {
            InputId = trade.InputId;

            TradeId = trade.TradeId;

            TradeType = trade.TradeType;

            Instrument = trade.Instrument;

            Quantity = trade.Quantity;

            Price = trade.Price;

            BuySell = trade.BuySell;

            CreatedOn = DateTime.Now;

            TradeStatus = trade.TradeStatus;

            IsActive = trade.IsActive;

            IsProcessed = trade.IsProcessed;

            Version = trade.Version;
        }
    }

    public class ProcessedTrade : TradeBase
    {  
        [Column("OldQuantity",Order =4)]
        public new double Quantity { get; set; }

        [Column(Order = 5)]
        public double NewQuantity { get; set; }

        [Column("OldPrice", Order =6)]
        public new double Price { get; set; }

        [Column(Order = 7)]
        public double NewPrice { get; set; }

        [Column(Order = 8)]
        public double TotalPrice { get; set; }

        [Column(Order = 14)]
        public bool IsIgnored { get; set; }


        public ProcessedTrade()
        {

        }

        public ProcessedTrade(Trade trade, double newQuantity, double newPrice)
        {
            InputId = trade.InputId;

            TradeId = trade.TradeId;

            TradeType = trade.TradeType;

            Instrument = trade.Instrument;
           
            Quantity = trade.Quantity;

            Price = trade.Price;

            NewQuantity = newQuantity;

            NewPrice = newPrice;

            TotalPrice = NewQuantity * NewPrice;

            BuySell = trade.BuySell;

            CreatedOn = DateTime.Now;

            TradeStatus = trade.TradeStatus;

            IsActive = trade.IsActive;

            IsProcessed = trade.IsProcessed;

            Version = trade.Version;
        }

    }
}
