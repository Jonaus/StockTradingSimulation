using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StockTradingSimulationAPI.ViewModels
{
    public class BalanceViewModel
    {
        [Required]
        public float Amount { get; set; }
    }
}