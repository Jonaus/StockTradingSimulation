using System.Web.Mvc;
using StockTradingSimulationWebClient.Core;

namespace StockTradingSimulationWebClient.Controllers
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