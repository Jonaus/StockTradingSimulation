using StockTradingSimulationWebClient.Core;
using StockTradingSimulationWebClient.Models;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace StockTradingSimulationWebClient.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult History()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Users()
        {
            ViewBag.Message = "Your contact page.";
            
            return View();
        }

        public ActionResult PositionsPartial(bool closed = false)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var stockPrices = Helpers.GetStockPrices(token);
            var positions = ApiClient.GetSelfPositions(token).Select(p => new PositionViewModel(p, stockPrices));

            ViewBag.Closed = closed;
            return PartialView("PartialViews/Positions", positions);
        }

        public ActionResult TransactionsPartial()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var transactions = ApiClient.GetHistory(token);

            return PartialView("PartialViews/Transactions", transactions);
        }

        public ActionResult NewPositionPartial()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var model = new NewPositionViewModel();
            model.AddStocks(token);

            return PartialView("PartialViews/NewPosition", model);
        }

        public string GetBalance()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            return ApiClient.GetSelfBalance(token).ToString("N");
        }

        public string GetRealBalance()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            return ApiClient.GetSelfRealBalance(token).ToString("N");
        }

        public float GetPositionEstimate(NewPositionViewModel model)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            return ApiClient.GetStockPrice(token, model.SelectedStockId) * model.Quantity;
        }
        
        public ActionResult OpenPosition(NewPositionViewModel model)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var realBalance = ApiClient.GetSelfRealBalance(token);
            var price = ApiClient.GetStockPrice(token, model.SelectedStockId) * model.Quantity;

            if (price > realBalance)
            {
                ModelState.AddModelError("Estimate", "Not enough money.");
                model.Success = false;
            }

            ApiClient.OpenPosition(token, model);
            model.AddStocks(token);
            
            return PartialView("PartialViews/NewPosition", model);
        }

        public ActionResult ClosePosition(int id)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            ApiClient.ClosePosition(token, id);

            return Json(new {id = 1});
        }
    }
}