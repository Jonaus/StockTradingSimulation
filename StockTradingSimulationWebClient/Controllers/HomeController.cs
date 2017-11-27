using StockTradingSimulationWebClient.Core;
using System.Security.Claims;
using System.Web.Mvc;

namespace StockTradingSimulationWebClient.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var token = ((ClaimsPrincipal) HttpContext.User).FindFirst("AccessToken").Value;
            var self = ApiClient.GetSelf(token);
            var balance = ApiClient.GetSelfBalance(token);

            ViewBag.Username = self != null ? self.UserName : "null";

            ViewBag.Balance = balance;

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
    }
}