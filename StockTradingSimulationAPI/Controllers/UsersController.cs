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
            return Db.Users;
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

            User user = await Db.Users.FirstOrDefaultAsync(u => u.Id == id);
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

            return Ok(await CalculateBalance(self.Id, false));
        }

        // GET: api/Users/5/Balance
        [Route("{id}/Balance")]
        [ResponseType(typeof(float))]
        public async Task<IHttpActionResult> GetUserBalance(string id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            User user = await Db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            return Ok(await CalculateBalance(user.Id, false));
        }

        // GET: api/Users/self/RealBalance
        [Route("self/RealBalance")]
        [ResponseType(typeof(float))]
        public async Task<IHttpActionResult> GetSelfRealBalance()
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            return Ok(await CalculateBalance(self.Id, true));
        }

        // GET: api/Users/5/RealBalance
        [Route("{id}/RealBalance")]
        [ResponseType(typeof(float))]
        public async Task<IHttpActionResult> GetUserRealBalance(string id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            User user = await Db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            return Ok(await CalculateBalance(user.Id, true));
        }

        // GET: api/Users/self/Watchlist
        [Route("self/Watchlist")]
        [ResponseType(typeof(IEnumerable<WatchedStock>))]
        public async Task<IHttpActionResult> GetSelfWatchlist()
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            var watchlist = await Db.WatchedStocks.Where(s => s.UserId == self.Id).ToListAsync();

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

            User user = await Db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            var watchlist = await Db.WatchedStocks.Where(s => s.UserId == user.Id).ToListAsync();

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

            var history = await Db.MoneyHistory.Where(s => s.UserId == self.Id).ToListAsync();

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

            User user = await Db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            var history = await Db.MoneyHistory.Where(s => s.UserId == user.Id).ToListAsync();

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

            User user = await Db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            var entry = new MoneyHistory
            {
                UserId = user.Id,
                Amount = model.Amount,
                Datetime = DateTime.UtcNow
            };
            Db.MoneyHistory.Add(entry);
            await Db.SaveChangesAsync();

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

            Db.Users.Remove(self);
            await Db.SaveChangesAsync();

            return Ok(self);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();
            
            User user = await Db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || (!IsAdmin() && user.Id != self.Id))
                return NotFound();

            Db.Users.Remove(user);
            await Db.SaveChangesAsync();

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
    }
}