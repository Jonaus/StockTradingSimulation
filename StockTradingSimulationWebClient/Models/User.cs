using System;
using Microsoft.AspNet.Identity.EntityFramework;
using StockTradingSimulationWebClient.Core;

namespace StockTradingSimulationWebClient.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisterDatetime { get; set; }

        public string Balance { get; set; }
        public string RealBalance { get; set; }

        public void CalcBalance(string token)
        {
            Balance = ApiClient.GetBalance(token, Id).ToString("N");
            RealBalance = ApiClient.GetRealBalance(token, Id).ToString("N");
        }
    }
}