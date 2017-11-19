using Microsoft.AspNet.Identity.EntityFramework;
using StockTradingSimulationAPI.Models;

namespace StockTradingSimulationAPI.Core
{
    public class MyUserStore : UserStore<User>
    {
        public MyUserStore() : base(new TradingContext())
        {
            
        }
    }
}