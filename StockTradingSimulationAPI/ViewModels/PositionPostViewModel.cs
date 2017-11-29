﻿using StockTradingSimulationAPI.Models;
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
        [Range(0, int.MaxValue, ErrorMessage = "Value cannot be negative !")]
        public int Quantity { get; set; }

        public float? Stoploss { get; set; }

        public Position Create(string userId, float stockPrice)
        {
            return new Position
            {
                UserId = userId,
                StockId = StockId,
                TransactionType = TransactionType,
                Quantity = Quantity,
                StartPrice = stockPrice,
                OpenDatetime = DateTime.UtcNow,
                Stoploss = Stoploss
            };
        }
    }
}