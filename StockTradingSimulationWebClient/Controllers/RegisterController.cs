using System.Web.Mvc;
using RestSharp;
using StockTradingSimulationWebClient.Core;
using StockTradingSimulationWebClient.Models;

namespace StockTradingSimulationWebClient.Controllers
{
    public class RegisterController : Controller
    {
        public ActionResult Index(string statusCode="")
        {
            ViewBag.Code = statusCode;
            return View();
        }

        [HttpPost]
        public ActionResult ProcessRegister(RegisterModel model)
        {
            var client = ApiClient.Client;
            var request = new RestRequest("api/Account/Register", Method.POST);
            request.AddObject(model);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
                return RedirectToAction("Index", "Login");
            
            return RedirectToAction("Index", "Register", new
            {
                statusCode = response.StatusCode
            });
        }
    }
}