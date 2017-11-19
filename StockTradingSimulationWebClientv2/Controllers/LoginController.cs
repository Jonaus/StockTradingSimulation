using RestSharp;
using StockTradingSimulationWebClientv2.Core;
using StockTradingSimulationWebClientv2.Models;
using System;
using System.Web.Mvc;
using HttpCookie = System.Web.HttpCookie;

namespace StockTradingSimulationWebClientv2.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index(string statusCode = "")
        {
            ViewBag.Code = statusCode;
            return View();
        }

        [HttpPost]
        public ActionResult ProcessLogin(LoginModel model)
        {
            var client = ApiClient.Client;
            var request = new RestRequest("oauth2/token", Method.POST);
            request.AddObject(model);
            request.AddParameter("grant_type", "password");

            IRestResponse<Token> response = client.Execute<Token>(request);
            if (!response.IsSuccessful)
            {
                return RedirectToAction("Index", "Login", new { statusCode = response.StatusCode });
            }

            HttpCookie tokenCookie = new HttpCookie("AccessToken", response.Data.access_token);
            tokenCookie.Expires = DateTime.UtcNow.AddSeconds(response.Data.expires_in);
            Response.Cookies.Add(tokenCookie);

            return RedirectToAction("Index", "Home");
        }
    }
}