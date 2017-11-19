using Microsoft.AspNet.Identity;
using StockTradingSimulationAPI.Models;

namespace StockTradingSimulationAPI.Core
{
    public class MyUserManager : UserManager<User>
    {
        public MyUserManager() : base(new MyUserStore())
        {
            
        }
    }
}