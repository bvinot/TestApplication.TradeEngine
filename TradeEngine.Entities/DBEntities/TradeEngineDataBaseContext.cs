using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TradeEngine.Entities.DBEntities
{
    public class TradeEngineDataBaseContext : DbContext
    {
        public TradeEngineDataBaseContext()
        {
            Database.Connection.ConnectionString = @"Data Source=BLRBVINOT\MSSQLSERVER01;Initial Catalog=TradeEngineDB;Integrated Security=true";
            Database.SetInitializer(new DropCreateDatabaseAlways<TradeEngineDataBaseContext>());
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TradeEngineDataBaseContext>());
        }

        public DbSet<Trade> TradesList { get; set; }

        public DbSet<OrphanTrade> OrphanTrades { get; set; }

        public DbSet<ProcessedTrade> ProcessedTrades { get; set; }
    }
}
