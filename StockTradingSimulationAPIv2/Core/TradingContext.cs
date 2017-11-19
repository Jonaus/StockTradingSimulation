using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using StockTradingSimulationAPI.Models;

namespace StockTradingSimulationAPI.Core
{
    public class TradingContext : IdentityDbContext<User>
    {
        public TradingContext() : base("TradingContext")
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<WatchedStock> WatchedStocks { get; set; }
        public DbSet<MoneyHistory> MoneyHistory { get; set; }
        public DbSet<Position> Positions { get; set; }
    }
}