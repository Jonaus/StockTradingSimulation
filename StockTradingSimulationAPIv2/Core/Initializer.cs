using System.Data.Entity;

namespace StockTradingSimulationAPI.Core
{
    public class Initializer : MigrateDatabaseToLatestVersion<TradingContext, Configuration>
    {
    }
}