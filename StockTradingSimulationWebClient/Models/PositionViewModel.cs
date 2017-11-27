using System.Globalization;
using StockTradingSimulationWebClient.Core;

namespace StockTradingSimulationWebClient.Models
{
    public class PositionViewModel
    {
        public Stock Stock { get; set; }
        public Transaction TransactionType { get; set; }
        public int Quantity { get; set; }
        public string StartPrice { get; set; }
        public string CurrentPrice { get; set; }
        public string Equity { get; set; }
        public string GainsPercent { get; set; }

        public PositionViewModel(Position position, string token)
        {
            var equity = Helpers.CalculatePositionEquity(position, token);
            Stock = position.Stock;
            TransactionType = position.TransactionType;
            Quantity = position.Quantity;
            StartPrice = position.StartPrice.ToString("N");
            CurrentPrice = ApiClient.GetStockPrice(token, position.StockId).ToString("N");
            Equity = equity.ToString("N");
            GainsPercent =
                ((equity / (position.StartPrice * position.Quantity) - 1) * 100).ToString("+0,0.00;-0,0.00;+0,0.00");
        }
    }
}