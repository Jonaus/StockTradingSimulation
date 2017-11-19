using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace StockTradingSimulationWebClientv2.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisterDatetime { get; set; }
    }
}