using StockTradingSimulationAPI.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace StockTradingSimulationAPI.ViewModels
{
    public class PositionPostViewModel
    {
        [Required]
        public int StockId { get; set; }

        [Required]
        public Transaction TransactionType { get; set; }

        [Required]
        public int Quantity { get; set; }

        public float? Stoploss { get; set; }

        public Position Create(string userId, float stockPrice)
        {
            Position position = new Position();
            position.UserId = userId;
            position.StockId = StockId;
            position.TransactionType = TransactionType;
            position.Quantity = Quantity;
            position.StartPrice = stockPrice;
            position.OpenDatetime = DateTime.Now;
            position.Stoploss = Stoploss;
            return position;
        }
    }
}