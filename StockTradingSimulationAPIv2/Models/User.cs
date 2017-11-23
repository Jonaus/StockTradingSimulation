using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace StockTradingSimulationAPI.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisterDatetime { get; } = DateTime.Now;
    }
    
    public class MoneyHistory
    {
        [Key]
        [Required]
        public int Id { get; private set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; private set; }

        [Required]
        public float Amount { get; set; }
        public DateTime Datetime { get; set; }
    }

    public class WatchedStock
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int StockId { get; set; }
        [ForeignKey("StockId")]
        public virtual Stock Stock { get; set; }
    }
}