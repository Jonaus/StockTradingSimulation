using System;
using System.Diagnostics.CodeAnalysis;

namespace StockTradingSimulationWebClient.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int StockId { get; set; }
        public virtual Stock Stock { get; set; }
        public Transaction TransactionType { get; set; }
        public int Quantity { get; set; }
        public float StartPrice { get; set; }
        public float ClosePrice { get; set; }
        public DateTime OpenDatetime { get; set; }
        public DateTime CloseDatetime { get; set; }
        public float Stoploss { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Transaction
    {
        BUY,
        SELL_SHORT
    }
}