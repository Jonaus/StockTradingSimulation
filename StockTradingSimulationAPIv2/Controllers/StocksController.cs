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

namespace StockTradingSimulationAPI.Controllers
{
    [RoutePrefix("api/Stocks")]
    public class StocksController : MyController
    {
        // GET: api/Stocks
        public IQueryable<Stock> GetStocks()
        {
            return db.Stocks;
        }

        // GET: api/Stocks/5
        [ResponseType(typeof(Stock))]
        public async Task<IHttpActionResult> GetStock(int id)
        {
            Stock stock = await db.Stocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            return Ok(stock);
        }

        // GET: api/Stocks/5/price
        [Route("{id}/price")]
        [ResponseType(typeof(float))]
        public async Task<IHttpActionResult> GetStockPrice(int id)
        {
            Stock stock = await db.Stocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            var price = await StockHelper.GetStockPrice(stock.Ticker);
            if (price < 0)
                return NotFound();

            return Ok(price);
        }

        // PUT: api/Stocks/5
        [ResponseType(typeof(void))]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> PutStock(int id, Stock stock)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != stock.Id)
                return BadRequest();

            db.Entry(stock).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockWithIdExists(id))
                    return NotFound();
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Stocks/5/watch
        [Route("{id}/watch")]
        public async Task<IHttpActionResult> WatchStock(int id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Stock stock = await db.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stock == null)
                return NotFound();

            if (await db.WatchedStocks.AnyAsync(s => s.UserId == self.Id && s.StockId == stock.Id))
                return StatusCode(HttpStatusCode.NoContent);
            
            db.WatchedStocks.Add(new WatchedStock
            {
                StockId = stock.Id,
                UserId = self.Id
            });
            await db.SaveChangesAsync();
            
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Stocks/5/unwatch
        [Route("{id}/unwatch")]
        public async Task<IHttpActionResult> UnwatchStock(int id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Stock stock = await db.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stock == null)
                return NotFound();

            WatchedStock wStock = await db.WatchedStocks.FirstOrDefaultAsync(s => s.UserId == self.Id && s.StockId == stock.Id);
            if (wStock == null)
                return NotFound();

            db.WatchedStocks.Remove(wStock);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Stocks
        [ResponseType(typeof(Stock))]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> PostStock(Stock stock)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.Stocks.Add(stock);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = stock.Id }, stock);
        }

        // DELETE: api/Stocks/5
        [ResponseType(typeof(Stock))]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> DeleteStock(int id)
        {
            Stock stock = await db.Stocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            db.Stocks.Remove(stock);
            await db.SaveChangesAsync();

            return Ok(stock);
        }
    }
}