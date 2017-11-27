using System.Linq;
using StockTradingSimulationWebClient.Core;
using System.Security.Claims;
using System.Web.Mvc;
using StockTradingSimulationWebClient.Models;

namespace StockTradingSimulationWebClient.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var token = ((ClaimsPrincipal) HttpContext.User).FindFirst("AccessToken").Value;
            var balance = ApiClient.GetSelfBalance(token);
            var positions = ApiClient.GetSelfPositions(token).Select(p => new PositionViewModel(p, token));
            
            ViewBag.Balance = balance.ToString("N");

            return View(positions);
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
    }
}