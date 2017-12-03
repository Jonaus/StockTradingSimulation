using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using StockTradingSimulationAPI.Helpers;

namespace StockTradingSimulationAPI.Models
{
    public class Stock
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string Ticker { get; set; }

        public async Task<float> GetCurrentPrice()
        {
            return await StockHelper.GetStockPrice(Ticker);
        }
    }
}