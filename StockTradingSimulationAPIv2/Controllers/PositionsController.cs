using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using StockTradingSimulationAPI.Core;
using StockTradingSimulationAPI.Helpers;
using StockTradingSimulationAPI.Models;
using StockTradingSimulationAPI.ViewModels;

namespace StockTradingSimulationAPI.Controllers
{
    [RoutePrefix("api/Positions")]
    public class PositionsController : MyController
    {
        // GET: api/Positions
        public async Task<IHttpActionResult> GetPositions()
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();
            
            if (IsAdmin())
                return Ok(await db.Positions.ToListAsync());

            return Ok(await db.Positions.Where(p => p.UserId == self.Id).ToListAsync());
        }

        // GET: api/Positions/5
        [ResponseType(typeof(Position))]
        public async Task<IHttpActionResult> GetPosition(int id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Position position = await db.Positions.FindAsync(id);
            if (position == null || (!IsAdmin() && position.UserId != self.Id))
                return NotFound();

            return Ok(position);
        }

        // PUT: api/Positions/5
        [ResponseType(typeof(void))]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> PutPosition(int id, PositionPutViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Position position = await db.Positions.FindAsync(id);
            if (position == null)
                return NotFound();

            model.AssignTo(position);
            db.Entry(position).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionWithIdExists(id))
                    return NotFound();
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Positions/5/close
        [Route("{id}/close")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ClosePosition(int id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Position position = await db.Positions.FirstOrDefaultAsync(p => p.Id == id && p.CloseDatetime == null);
            if (position == null)
                return NotFound();

            if (position.UserId != self.Id && !IsAdmin())
                return Unauthorized();

            await position.Close();
            db.Entry(position).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionWithIdExists(id))
                    return NotFound();
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Positions
        [ResponseType(typeof(Position))]
        public async Task<IHttpActionResult> PostPosition(PositionPostViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Stock stock = await db.Stocks.FirstOrDefaultAsync(s => s.Id == model.StockId);
            if (stock == null)
                return NotFound();

            var stockPrice = await stock.GetCurrentPrice();
            if (stockPrice < 0)
                return NotFound();

            var position = model.Create(self.Id, stockPrice);
            
            db.Positions.Add(position);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = position.Id }, position);
        }

        // DELETE: api/Positions/5
        [ResponseType(typeof(Position))]
        public async Task<IHttpActionResult> DeletePosition(int id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Position position = await db.Positions.FindAsync(id);
            if (position == null || (!IsAdmin() && position.UserId != self.Id))
                return NotFound();

            db.Positions.Remove(position);
            await db.SaveChangesAsync();

            return Ok(position);
        }
    }
}