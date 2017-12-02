using System.ComponentModel.DataAnnotations;

namespace StockTradingSimulationWebClient.Models
{
    public class Stock
    {
        public int Id { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string Ticker { get; set; }
    }
}