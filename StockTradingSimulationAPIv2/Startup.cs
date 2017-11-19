using Microsoft.Owin;
using Owin;
using StockTradingSimulationAPI;

[assembly: OwinStartup(typeof(Startup))]

namespace StockTradingSimulationAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }
    }
}