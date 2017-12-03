using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradingSimulationAPI.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisterDatetime { get; set; }
    }
    
    public class MoneyHistory
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; protected set; }

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