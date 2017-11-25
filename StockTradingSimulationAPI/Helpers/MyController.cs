using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using StockTradingSimulationAPI.Core;
using StockTradingSimulationAPI.Models;

namespace StockTradingSimulationAPI.Helpers
{
    public class MyController : ApiController
    {
        protected TradingContext Db = new TradingContext();

        protected async Task<User> SelfUser()
        {
            var name = HttpContext.Current.User.Identity.GetUserName();
            if (name == null) return null;
            return await Db.Users.FirstOrDefaultAsync(u => u.UserName == name);
        }

        protected bool IsAdmin()
        {
            return User.IsInRole(Roles.Admin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected bool UserWithNameExists(string name)
        {
            return Db.Users.Count(e => e.UserName == name) > 0;
        }

        protected bool UserWithIdExists(string id)
        {
            return Db.Users.Count(e => e.Id == id) > 0;
        }

        protected bool StockWithIdExists(int id)
        {
            return Db.Stocks.Count(e => e.Id == id) > 0;
        }

        protected bool PositionWithIdExists(int id)
        {
            return Db.Positions.Count(e => e.Id == id) > 0;
        }
    }
}