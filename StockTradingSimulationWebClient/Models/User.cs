using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace StockTradingSimulationWebClient.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisterDatetime { get; set; }
    }
}