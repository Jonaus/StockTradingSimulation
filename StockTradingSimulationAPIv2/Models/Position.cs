using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace StockTradingSimulationAPI.Models
{
    public class Position
    {
        [Key]
        [Required]
        public int Id { get; private set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; private set; }

        [Required]
        public int StockId { get; set; }
        [ForeignKey("StockId")]
        public virtual Stock Stock { get; private set; }

        [Required]
        public Transaction TransactionType { get; set; }
        [Required]
        public int Quantity { get; set; }
        public float StartPrice { get; private set; }
        public float? ClosePrice { get; private set; }
        public DateTime OpenDatetime { get; private set; }
        public DateTime? CloseDatetime { get; private set; }
        public float? Stoploss { get; set; }

        public void SetStartPrice(float price)
        {
            StartPrice = price;
        }

        public void SetOpenDatetime()
        {
            OpenDatetime = DateTime.Now;
        }

        public async Task Close()
        {
            CloseDatetime = DateTime.Now;
            ClosePrice = await Stock.GetCurrentPrice();
        }
    }

    public enum Transaction
    {
        BUY,
        SELL_SHORT
    }
}