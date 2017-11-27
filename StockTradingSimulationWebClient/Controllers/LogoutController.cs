using System.Web;
using System.Web.Mvc;

namespace StockTradingSimulationWebClient.Controllers
{
    public class LogoutController : Controller
    {
        public ActionResult Index()
        {
            Request.GetOwinContext().Authentication.SignOut("ApplicationCookie");

            return RedirectToAction("Index", "Login");
        }
    }
}