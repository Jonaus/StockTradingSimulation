using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(StockTradingSimulationWebClient.Startup))]

namespace StockTradingSimulationWebClient
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions()
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Login/Index")
            });
        }
    }
}
