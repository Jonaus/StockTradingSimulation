using NUnit.Framework;
using NUnit.Framework.Internal;
using StockTradingSimulationAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockTradingSimulationAPI.Tests.Tests
{
    [TestFixture()]
    class StockTesting : TestBase
    {
        private Stock _stock;
        private const string StockName = "NunitTestStock";

        [SetUp]
        public void Setup()
        {
            _stock = APIClient.Post<Stock>(AdminToken, "api/stocks", new
            {
                Ticker = "TSLA",
                Fullname = StockName
            });
        }

        [TearDown]
        public void Teardown() { APIClient.Delete<Stock>(AdminToken, $"api/stocks/{_stock.Id}"); }

        [TestCase("SNAP", "snapchat", ExpectedResult = true)]
        [TestCase("FB", "facebook", ExpectedResult = true)]
        [TestCase("FB", "", ExpectedResult = false)]
        public bool StockAddTest(string ticker, string fullname)
        {
            var createdStock = APIClient.Post<Stock>(AdminToken, "api/stocks", new
            {
                Ticker = ticker,
                Fullname = fullname
            });
            if (createdStock == null) return false;

            var foundStock = APIClient.Get<Stock>(AdminToken, $"api/stocks/{createdStock.Id}");
            if (foundStock == null) return false;

            APIClient.Delete<Stock>(AdminToken, $"api/stocks/{createdStock.Id}");

            return true;
        }

        [TestCase(StockName, ExpectedResult = true)]
        [TestCase("something", ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        public bool StockDeleteTest(string fullname)
        {
            var foundStock = APIClient.Get<IEnumerable<Stock>>(AdminToken, "api/stocks")
                .FirstOrDefault(s => s.Fullname == fullname);
            if (foundStock == null) return false;

            APIClient.Delete<Stock>(AdminToken, $"api/stocks/{foundStock.Id}");

            var foundStock2 = APIClient.Get<IEnumerable<Stock>>(AdminToken, "api/stocks")
                .FirstOrDefault(s => s.Fullname == fullname);
            return foundStock2 == null;
        }

        [TestCase(StockName, "TWTR", "TWITTER2", ExpectedResult = true)]
        [TestCase(StockName, "TWTR", "", ExpectedResult = false)]
        [TestCase("something", "TSLA", "TESLA", ExpectedResult = false)]
        public bool StockEditTest(string fullname, string ticker, string newName)
        {
            var foundStock = APIClient.Get<IEnumerable<Stock>>(AdminToken, "api/stocks")
                .FirstOrDefault(s => s.Fullname == fullname);
            if (foundStock == null) return false;

            APIClient.Put<Stock>(AdminToken, $"api/stocks/{foundStock.Id}", new
            {
                Ticker = ticker,
                Fullname = newName
            });

            var foundStock2 = APIClient.Get<IEnumerable<Stock>>(AdminToken, "api/stocks")
                .FirstOrDefault(s => s.Fullname == newName);
            if (foundStock2 == null) return false;

            return foundStock2.Ticker == ticker && foundStock2.Fullname == newName;
        }
    }
}
