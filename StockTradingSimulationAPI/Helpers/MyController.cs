using System;
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

        protected async Task<float> CalculateBalance(string userId, bool realBalance)
        {
            var history = Db.MoneyHistory
                .Where(h => h.UserId == userId && h.Datetime <= DateTime.UtcNow);
            var positions = Db.Positions
                .Where(p => p.UserId == userId)
                .AsEnumerable()
                .Select(async p =>
                {
                    float price;
                    float result;
                    if (p.CloseDatetime == null)
                        price = await p.Stock.GetCurrentPrice();
                    else
                        price = p.ClosePrice ?? await p.Stock.GetCurrentPrice();
                    if (p.TransactionType == Transaction.BUY)
                        result = p.Quantity * price;
                    else if (p.TransactionType == Transaction.SELL_SHORT)
                        result = p.Quantity * (2 * p.StartPrice - price);
                    else
                        throw new Exception("Unsupported transaction type.");
                    if (realBalance)
                        return p.CloseDatetime == null ? p.Quantity * p.StartPrice * -1 : p.Quantity * price;
                    return p.CloseDatetime == null ? result - p.Quantity * p.StartPrice : result;
                });
            var historySum = history.Any() ? history.Sum(h => h.Amount) : 0f;
            var positionsResults = await Task.WhenAll(positions);
            var positionsSum = positionsResults.Any() ? positionsResults.Sum() : 0f;
            return historySum + positionsSum;
        }
    }
}