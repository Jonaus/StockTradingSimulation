using System.Web.Mvc;
using StockTradingSimulationWebClient.Core;

namespace StockTradingSimulationWebClient.Controllers
{
    public class ApiController : Controller
    {
        [HttpPost]
        public ActionResult ChangePort(int newPort)
        {
            ApiClient.ChangePort(newPort);
            return Redirect(Request.UrlReferrer?.ToString());
        }
    }
}