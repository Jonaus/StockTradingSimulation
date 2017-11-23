using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using StockTradingSimulationAPI.Models;

namespace StockTradingSimulationAPI.ViewModels
{
    public class PositionPutViewModel
    {
        public string UserId { get; set; }
        public int? StockId { get; set; }
        public Transaction? TransactionType { get; set; }
        public int? Quantity { get; set; }
        public float? StartPrice { get; set; }
        public float? ClosePrice { get; set; }
        public DateTime? OpenDatetime { get; set; }
        public DateTime? CloseDatetime { get; set; }
        public float? Stoploss { get; set; }

        public void AssignTo(Position position)
        {
            position.UserId = UserId ?? position.UserId;
            position.StockId = StockId ?? position.StockId;
            position.TransactionType = TransactionType ?? position.TransactionType;
            position.Quantity = Quantity ?? position.Quantity;
            position.StartPrice = StartPrice ?? position.StartPrice;
            position.ClosePrice = ClosePrice ?? position.ClosePrice;
            position.OpenDatetime = OpenDatetime ?? position.OpenDatetime;
            position.CloseDatetime = CloseDatetime ?? position.CloseDatetime;
            position.Stoploss = Stoploss ?? position.Stoploss;
        }
    }
}