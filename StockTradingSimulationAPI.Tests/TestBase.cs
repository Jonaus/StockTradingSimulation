using NUnit.Framework;
using NUnit.Framework.Internal;
using StockTradingSimulationAPI.Tests.Models;

namespace StockTradingSimulationAPI.Tests
{
    [TestFixture]
    public class TestBase
    {
        protected string AdminToken;

        [OneTimeSetUp]
        public void SetUp()
        {
            AdminToken = APIClient.Post<Token>("", "api/account/login", new
            {
                Username = "administrator",
                Password = "administrator123"
            }).access_token;
        }
    }
}
