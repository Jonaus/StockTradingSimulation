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
    [RoutePrefix("api/Stocks")]
    public class StocksController : MyController
    {
        // GET: api/Stocks
        public IQueryable<Stock> GetStocks()
        {
            return Db.Stocks;
        }

        // GET: api/Stocks/5
        [ResponseType(typeof(Stock))]
        public async Task<IHttpActionResult> GetStock(int id)
        {
            Stock stock = await Db.Stocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            return Ok(stock);
        }

        // GET: api/Stocks/5/price
        [Route("{id}/price")]
        [ResponseType(typeof(float))]
        public async Task<IHttpActionResult> GetStockPrice(int id)
        {
            Stock stock = await Db.Stocks.FindAsync(id);
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
        public async Task<IHttpActionResult> PutStock(int id, StockViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Stock stock = await Db.Stocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            model.AssignTo(stock);

            Db.Entry(stock).State = EntityState.Modified;

            try
            {
                await Db.SaveChangesAsync();
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

            Stock stock = await Db.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stock == null)
                return NotFound();

            if (await Db.WatchedStocks.AnyAsync(s => s.UserId == self.Id && s.StockId == stock.Id))
                return StatusCode(HttpStatusCode.NoContent);
            
            Db.WatchedStocks.Add(new WatchedStock
            {
                StockId = stock.Id,
                UserId = self.Id
            });
            await Db.SaveChangesAsync();
            
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Stocks/5/unwatch
        [Route("{id}/unwatch")]
        public async Task<IHttpActionResult> UnwatchStock(int id)
        {
            var self = await SelfUser();
            if (self == null)
                return Unauthorized();

            Stock stock = await Db.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stock == null)
                return NotFound();

            WatchedStock wStock = await Db.WatchedStocks.FirstOrDefaultAsync(s => s.UserId == self.Id && s.StockId == stock.Id);
            if (wStock == null)
                return NotFound();

            Db.WatchedStocks.Remove(wStock);
            await Db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Stocks
        [ResponseType(typeof(Stock))]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> PostStock(Stock stock)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Db.Stocks.Add(stock);
            await Db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = stock.Id }, stock);
        }

        // DELETE: api/Stocks/5
        [ResponseType(typeof(Stock))]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IHttpActionResult> DeleteStock(int id)
        {
            Stock stock = await Db.Stocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            var positions = Db.Positions.Where(p => p.StockId == stock.Id);
            Db.Positions.RemoveRange(positions);

            var watchedStocks = Db.WatchedStocks.Where(s => s.StockId == stock.Id);
            Db.WatchedStocks.RemoveRange(watchedStocks);

            Db.Stocks.Remove(stock);
            await Db.SaveChangesAsync();

            return Ok(stock);
        }
    }
}