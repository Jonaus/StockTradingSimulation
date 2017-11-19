using System;
using System.Web.Mvc;

namespace StockTradingSimulationWebClientv2.Controllers
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