using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity.EntityFramework;
using StockTradingSimulationAPI.Core;
using StockTradingSimulationAPI.Helpers;
using StockTradingSimulationAPI.Models;
using StockTradingSimulationAPI.ViewModels;

namespace StockTradingSimulationAPI.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : MyController
    {
        // POST: api/Account/Register
        [Route("Register")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Register(RegisterViewModel info)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var user = Db.Users.Add(new User
            {
                UserName = info.Username,
                Email = info.Email,
                RegisterDatetime = DateTime.UtcNow
            });

            var userRole = await Db.Roles.FirstOrDefaultAsync(r => r.Name == Roles.User);
            user.Roles.Add(new IdentityUserRole { RoleId = userRole.Id });
            await new MyUserStore().SetPasswordHashAsync(user, new MyUserManager().PasswordHasher.HashPassword(info.Password));

            try
            {
                await Db.SaveChangesAsync();
                var entry = new MoneyHistory
                {
                    UserId = user.Id,
                    Amount = 10000,
                    Datetime = DateTime.UtcNow
                };
                Db.MoneyHistory.Add(entry);
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserWithIdExists(user.Id))
                    return Conflict();
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { controller = "Users", id = user.Id }, user);
        }
    }
}