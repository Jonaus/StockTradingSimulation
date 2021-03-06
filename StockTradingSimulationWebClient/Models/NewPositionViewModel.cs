﻿using StockTradingSimulationWebClient.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace StockTradingSimulationWebClient.Models
{
    public class NewPositionViewModel
    {
        [Display(Name = "Stock")]
        public int SelectedStockId { get; set; }

        [Display(Name = "Type")]
        public int SelectedTransactionId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be higher than 0!")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Estimate")]
        public float Estimate { get; set; }

        public bool Success { get; set; } = true;

        public IEnumerable<SelectListItem> Stocks { get; private set; }
        public IEnumerable<SelectListItem> TransactionTypes { get; }

        public NewPositionViewModel()
        {
            var transactions = Enum.GetValues(typeof(Transaction)).Cast<Transaction>().Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = t.ToString()
            });
            
            TransactionTypes = new SelectList(transactions, "Value", "Text");
        }

        public void AddStocks(string token)
        {
            var stocks = ApiClient.GetStocks(token).Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.Ticker} ({s.Fullname})"
            });

            Stocks = new SelectList(stocks, "Value", "Text");
        }
    }
}