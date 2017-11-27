using Microsoft.Owin.Security;
using RestSharp;
using StockTradingSimulationWebClient.Core;
using StockTradingSimulationWebClient.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace StockTradingSimulationWebClient.Controllers
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
            var request = new RestRequest("api/account/login", Method.POST);
            request.AddObject(model);

            IRestResponse<Token> response = client.Execute<Token>(request);
            if (!response.IsSuccessful)
            {
                return RedirectToAction("Index", "Login", new { statusCode = response.StatusCode });
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(response.Data.access_token) as JwtSecurityToken;
            var roleClaims = token.Claims
                .Where(c => c.Type == "role")
                .Select(c => new Claim(ClaimTypes.Role, c.Value));

            AuthenticationProperties options = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = token.ValidTo
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim("AccessToken", token.RawData)
            };
            claims.AddRange(roleClaims);
            
            var identity = new ClaimsIdentity(claims, "ApplicationCookie");

            Request.GetOwinContext().Authentication.SignIn(options, identity);

            return RedirectToAction("Index", "Home");
        }
    }
}