﻿using System;
using StockTradingSimulationWebClient.Core;
using System.Collections.Generic;
using System.Linq;

namespace StockTradingSimulationWebClient.Models
{
    public class PositionViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public Stock Stock { get; set; }
        public Transaction TransactionType { get; set; }
        public int Quantity { get; set; }
        public string StartPrice { get; set; }
        public bool Closed { get; set; }
        public DateTime ClosedDatetime { get; set; }
        public string CurrentPrice { get; set; }
        public string ClosedPrice { get; set; }
        public string Equity { get; set; }
        public string Gains { get; set; }
        public string GainsPercent { get; set; }

        public PositionViewModel(Position position, IEnumerable<StockPrice> stockPrices)
        {
            var cPrice = stockPrices.FirstOrDefault(s => s.StockId == position.StockId)?.Price ?? -1;

            var equity = Helpers.CalculatePositionEquity(position, cPrice);
            Id = position.Id;
            UserId = position.UserId;
            Stock = position.Stock;
            TransactionType = position.TransactionType;
            Quantity = position.Quantity;
            Closed = position.CloseDatetime > position.OpenDatetime;
            ClosedDatetime = position.CloseDatetime;
            StartPrice = position.StartPrice.ToString("N");
            CurrentPrice = cPrice.ToString("N");
            ClosedPrice = position.ClosePrice.ToString("N");
            Equity = equity.ToString("N");
            Gains = (equity - position.StartPrice * position.Quantity).ToString("+$0,0.00;-$0,0.00;+$0,0.00");
            GainsPercent =
                ((equity / (position.StartPrice * position.Quantity) - 1) * 100).ToString("+0,0.00;-0,0.00;+0,0.00");
        }
    }
}