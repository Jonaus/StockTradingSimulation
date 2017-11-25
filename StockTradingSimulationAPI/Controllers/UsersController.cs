using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using StockTradingSimulationAPI.Core;
using StockTradingSimulationAPI.Helpers;
using StockTradingSimulationAPI.Models;
using StockTradingSimulationAPI.ViewModels;

namespace StockTradingSimulationAPI.Controllers
{
    [RoutePrefix("api/Users")]
    public class UsersController : MyController
    {
        // GET: api/Users
        [Authorize(Roles = Roles.Admin)]
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/self
        [Route("self")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetSelf()
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            return Ok(self);
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            return Ok(user);
        }

        // GET: api/Users/self/Balance
        [Route("self/Balance")]
        [ResponseType(typeof(float))]
        public async Task<IHttpActionResult> GetSelfBalance()
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            return Ok(await CalculateBalance(self.Id));
        }

        // GET: api/Users/5/Balance
        [Route("{id}/Balance")]
        [ResponseType(typeof(float))]
        public async Task<IHttpActionResult> GetUserBalance(string id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            return Ok(await CalculateBalance(user.Id));
        }

        // GET: api/Users/self/Watchlist
        [Route("self/Watchlist")]
        [ResponseType(typeof(IEnumerable<WatchedStock>))]
        public async Task<IHttpActionResult> GetSelfWatchlist()
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            var watchlist = await db.WatchedStocks.Where(s => s.UserId == self.Id).ToListAsync();

            return Ok(watchlist);
        }

        // GET: api/Users/5/Watchlist
        [Route("{id}/Watchlist")]
        [ResponseType(typeof(IEnumerable<WatchedStock>))]
        public async Task<IHttpActionResult> GetUserWatchlist(string id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            var watchlist = await db.WatchedStocks.Where(s => s.UserId == user.Id).ToListAsync();

            return Ok(watchlist);
        }

        // GET: api/Users/self/History
        [Route("self/History")]
        [ResponseType(typeof(IEnumerable<MoneyHistory>))]
        public async Task<IHttpActionResult> GetSelfHistory()
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            var history = await db.MoneyHistory.Where(s => s.UserId == self.Id).ToListAsync();

            return Ok(history);
        }

        // GET: api/Users/5/History
        [Route("{id}/History")]
        [ResponseType(typeof(IEnumerable<MoneyHistory>))]
        public async Task<IHttpActionResult> GetUserHistory(string id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            var history = await db.MoneyHistory.Where(s => s.UserId == user.Id).ToListAsync();

            return Ok(history);
        }

        // POST: api/Users/5/Balance
        [Route("{id}/Balance")]
        [ResponseType(typeof(MoneyHistory))]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> PostBalance(string id, BalanceViewModel model)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            var entry = new MoneyHistory
            {
                UserId = user.Id,
                Amount = model.Amount,
                Datetime = DateTime.Now
            };
            db.MoneyHistory.Add(entry);
            await db.SaveChangesAsync();

            return Ok(entry);
        }

        // DELETE: api/Users/self
        [Route("self")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteSelf()
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            db.Users.Remove(self);
            await db.SaveChangesAsync();

            return Ok(self);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();
            
            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        #region UNUSED ENDPOINTS

        // PUT: api/Users/self
        //[Route("self")]
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutSelf(User user)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var self = await SelfUser();
        //    if (self == null)
        //        return Unauthorized();

        //    if (UserWithIdExists(user.Id) && user.Id != self.Id && !self.IsAdmin())
        //        return Unauthorized();

        //    db.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserWithIdExists(self.Id))
        //            return NotFound();
        //        throw;
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// PUT: api/Users/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutUser(string id, User user)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    if (id != user.Id)
        //        return BadRequest();

        //    var self = await SelfUser();
        //    if (self == null)
        //        return Unauthorized();

        //    if (UserWithIdExists(user.Id) && user.Id != self.Id && !self.IsAdmin())
        //        return Unauthorized();

        //    db.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserWithIdExists(id))
        //            return NotFound();
        //        throw;
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/Users
        //[ResponseType(typeof(User))]
        //public async Task<IHttpActionResult> PostUser(User user)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    user.Roles.Add(new IdentityUserRole {RoleId = Roles.User});
        //    var store = new MyUserStore();
        //    await store.SetPasswordHashAsync(user, new MyUserManager().PasswordHasher.HashPassword(user.PasswordHash));

        //    db.Users.Add(user);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //        var entry = new MoneyHistory
        //        {
        //            UserId = user.Id,
        //            Amount = 10000,
        //            Datetime = DateTime.Now
        //        };
        //        db.MoneyHistory.Add(entry);
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (UserWithIdExists(user.Id))
        //            return Conflict();
        //        throw;
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        //}

        #endregion

        private async Task<float> CalculateBalance(string userId)
        {
            var history = db.MoneyHistory
                .Where(h => h.UserId == userId && h.Datetime <= DateTime.Now);
            var positions = db.Positions
                .Where(p => p.UserId == userId)
                .AsEnumerable()
                .Select(async p =>
                {
                    float price = 0;
                    if (p.CloseDatetime != null)
                        price = await p.Stock.GetCurrentPrice();
                    else
                        price = p.StartPrice;
                    if (p.TransactionType == Transaction.BUY)
                        return p.Quantity * price;
                    if (p.TransactionType == Transaction.SELL_SHORT)
                        return (p.StartPrice * 2 - price) * 100;
                    throw new Exception("Unsupported transaction type.");
                });
            var historySum = history.Any() ? history.Sum(h => h.Amount) : 0f;
            var positionsResults = await Task.WhenAll(positions);
            var positionsSum = positionsResults.Any() ? positionsResults.Sum() : 0f;
            return historySum + positionsSum;
        }
    }
}