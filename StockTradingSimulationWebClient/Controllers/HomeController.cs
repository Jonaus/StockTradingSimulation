using System.Web.Mvc;
using StockTradingSimulationWebClient.Core;

namespace StockTradingSimulationWebClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var token = Request.Cookies.Get("AccessToken")?.Value;
            var self = ApiClient.GetSelf(token);
            var balance = ApiClient.GetSelfBalance(token);

            ViewBag.Username = self != null ? self.UserName : "null";

            ViewBag.Balance = balance;

            return View();
        }

        public ActionResult History()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Users()
        {
            ViewBag.Message = "Your contact page.";
            
            return View();
        }
    }
}