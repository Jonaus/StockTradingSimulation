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
        protected TradingContext db = new TradingContext();

        protected async Task<User> SelfUser()
        {
            var name = HttpContext.Current.User.Identity.GetUserName();
            if (name == null) return null;
            return await db.Users.FirstOrDefaultAsync(u => u.UserName == name);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected bool UserWithNameExists(string name)
        {
            return db.Users.Count(e => e.UserName == name) > 0;
        }

        protected bool UserWithIdExists(string id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }

        protected bool StockWithIdExists(int id)
        {
            return db.Stocks.Count(e => e.Id == id) > 0;
        }

        protected bool PositionWithIdExists(int id)
        {
            return db.Positions.Count(e => e.Id == id) > 0;
        }
    }
}