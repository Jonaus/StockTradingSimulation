using System;

namespace StockTradingSimulationWebClient.Models
{
    public class MoneyHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public float Amount { get; set; }
        public DateTime Datetime { get; set; }
    }
}