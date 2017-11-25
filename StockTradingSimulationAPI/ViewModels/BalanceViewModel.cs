using System.ComponentModel.DataAnnotations;

namespace StockTradingSimulationAPI.ViewModels
{
    public class BalanceViewModel
    {
        [Required]
        public float Amount { get; set; }
    }
}