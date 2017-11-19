using StockTradingSimulationWebClientv2.Core;
using System.Web.Mvc;

namespace StockTradingSimulationWebClientv2.Controllers
{
    public class APIController : Controller
    {
        [HttpPost]
        public ActionResult ChangePort(int newPort)
        {
            ApiClient.ChangePort((int)newPort);
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}