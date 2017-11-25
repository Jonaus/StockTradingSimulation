using System;
using System.Web.Mvc;

namespace StockTradingSimulationWebClient.Controllers
{
    public class LogoutController : Controller
    {
        public ActionResult Index()
        {
            Response.Cookies["AccessToken"].Expires = DateTime.Now.AddDays(-1);

            return RedirectToAction("Index", "Login");
        }
    }
}