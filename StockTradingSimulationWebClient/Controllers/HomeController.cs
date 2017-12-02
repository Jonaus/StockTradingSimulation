using StockTradingSimulationWebClient.Core;
using StockTradingSimulationWebClient.Models;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

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
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Users()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult UserEdit(string userId)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var user = ApiClient.GetUser(token, userId);

            return View(user);
        }

        [Authorize]
        public ActionResult PositionsPartial(bool closed = false)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var stockPrices = Helpers.GetStockPrices(token);
            var self = ApiClient.GetSelf(token);
            var positions = ApiClient.GetSelfPositions(token)
                .Where(p => p.UserId == self.Id)
                .Select(p => new PositionViewModel(p, stockPrices));

            ViewBag.Closed = closed;
            return PartialView("PartialViews/Positions", positions);
        }

        [Authorize]
        public ActionResult TransactionsPartial()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var transactions = ApiClient.GetHistory(token);

            return PartialView("PartialViews/Transactions", transactions);
        }

        [Authorize]
        public ActionResult NewPositionPartial()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var model = new NewPositionViewModel();
            model.AddStocks(token);

            return PartialView("PartialViews/NewPosition", model);
        }

        [Authorize]
        public ActionResult MoneySendPartial(string userId)
        {
            var model = new TransactionViewModel();
            model.UserId = userId;

            return PartialView("PartialViews/MoneySendPartial", model);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult UsersPartial()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            var users = ApiClient.GetUsers(token);
            users.ForEach(u => u.CalcBalance(token));

            return PartialView("PartialViews/UsersPartial", users);
        }

        [Authorize]
        public string GetBalance()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            return ApiClient.GetSelfBalance(token).ToString("N");
        }

        [Authorize]
        public string GetRealBalance()
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            return ApiClient.GetSelfRealBalance(token).ToString("N");
        }

        [Authorize]
        public float GetPositionEstimate(NewPositionViewModel model)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            return ApiClient.GetStockPrice(token, model.SelectedStockId) * model.Quantity;
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult SendMoney(TransactionViewModel model)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            ApiClient.SendMoney(token, model);

            return PartialView("PartialViews/MoneySendPartial", model);
        }

        [Authorize]
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

        [Authorize]
        public ActionResult ClosePosition(int id)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            ApiClient.ClosePosition(token, id);

            return Json(new {id = 1});
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteUser(string userId)
        {
            var token = ((ClaimsPrincipal)HttpContext.User).FindFirst("AccessToken").Value;
            ApiClient.DeleteUser(token, userId);

            return RedirectToAction("Users");
        }
    }
}