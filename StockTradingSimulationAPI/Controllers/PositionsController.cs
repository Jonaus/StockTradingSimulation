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
                return Ok(await Db.Positions.ToListAsync());

            return Ok(await Db.Positions.Where(p => p.UserId == self.Id).ToListAsync());
        }

        // GET: api/Positions/5
        [ResponseType(typeof(Position))]
        public async Task<IHttpActionResult> GetPosition(int id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Position position = await Db.Positions.FindAsync(id);
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

            Position position = await Db.Positions.FindAsync(id);
            if (position == null)
                return NotFound();

            model.AssignTo(position);
            Db.Entry(position).State = EntityState.Modified;

            try
            {
                await Db.SaveChangesAsync();
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

            Position position = await Db.Positions.FirstOrDefaultAsync(p => p.Id == id && p.CloseDatetime == null);
            if (position == null)
                return NotFound();

            if (position.UserId != self.Id && !IsAdmin())
                return Unauthorized();

            await position.Close();
            Db.Entry(position).State = EntityState.Modified;

            try
            {
                await Db.SaveChangesAsync();
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
            
            Stock stock = await Db.Stocks.FirstOrDefaultAsync(s => s.Id == model.StockId);
            if (stock == null)
                return NotFound();

            var stockPrice = await stock.GetCurrentPrice();
            if (stockPrice < 0)
                return NotFound();

            if (stockPrice * model.Quantity > await CalculateBalance(self.Id, true))
                return BadRequest("Not enough money.");

            var position = model.Create(self.Id, stockPrice);
            
            Db.Positions.Add(position);
            await Db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = position.Id }, position);
        }

        // DELETE: api/Positions/5
        [ResponseType(typeof(Position))]
        public async Task<IHttpActionResult> DeletePosition(int id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Position position = await Db.Positions.FindAsync(id);
            if (position == null || (!IsAdmin() && position.UserId != self.Id))
                return NotFound();

            Db.Positions.Remove(position);
            await Db.SaveChangesAsync();

            return Ok(position);
        }
    }
}