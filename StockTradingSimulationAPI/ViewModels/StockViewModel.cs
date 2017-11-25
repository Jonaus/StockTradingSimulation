using StockTradingSimulationAPI.Models;

namespace StockTradingSimulationAPI.ViewModels
{
    public class StockViewModel
    {
        public string Fullname { get; set; }
        public string Ticker { get; set; }

        public void AssignTo(Stock stock)
        {
            stock.Fullname = Fullname ?? stock.Fullname;
            stock.Ticker = Ticker ?? stock.Ticker;
        }
    }
}