using NUnit.Framework;
using NUnit.Framework.Internal;
using StockTradingSimulationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockTradingSimulationAPI.Tests.Tests
{
    [TestFixture()]
    class PositionTests : TestBase
    {
        private Stock _stock;
        private Position _position;
        public const string TestStockname = "NunitTestStock";

        [SetUp]
        public void Setup()
        {
            _stock = APIClient.Post<Stock>(AdminToken, "api/stocks", new
            {
                Ticker = "AAPL",
                Fullname = TestStockname
            });

            _position = APIClient.Post<Position>(AdminToken, "api/positions", new
            {
                StockId = _stock.Id,
                TransactionType = Transaction.BUY,
                Quantity = 1
            });
        }

        [TearDown]
        public void Teardown()
        {
            APIClient.Delete<string>(AdminToken, $"api/positions/{_position.Id}");
            APIClient.Delete<string>(AdminToken, $"api/stocks/{_stock.Id}");
        }

        [TestCase(TestStockname, Transaction.BUY, 2, ExpectedResult = true)]
        [TestCase("AAPLLLLL", Transaction.BUY, 2, ExpectedResult = false)]
        [TestCase(TestStockname, Transaction.SELL_SHORT, 0, ExpectedResult = false)]
        public bool PositionAddTest(string stockName, Transaction transactionType, int quantity)
        {
            var stock = APIClient.Get<IEnumerable<Stock>>(AdminToken, "api/stocks")
                .FirstOrDefault(s => s.Fullname == stockName);
            if (stock == null) return false;

            var createdPosition = APIClient.Post<Position>(AdminToken, "api/positions", new
            {
                StockId = stock.Id,
                TransactionType = transactionType,
                Quantity = quantity
            });
            if (createdPosition == null) return false;

            var foundPosition = APIClient.Get<Position>(AdminToken, $"api/positions/{createdPosition.Id}");
            APIClient.Delete<string>(AdminToken, $"api/positions/{createdPosition.Id}");

            return foundPosition != null;
        }

        [TestCase(50, 3, ExpectedResult = true)]
        [TestCase(60, 2, ExpectedResult = true)]
        public bool PositionEditTest(float startPrice, int quantity)
        {
            APIClient.Put<string>(AdminToken, $"api/positions/{_position.Id}", new
            {
                StartPrice = startPrice,
                Quantity = quantity
            });
            var position = APIClient.Get<Position>(AdminToken, $"api/positions/{_position.Id}");

            return Math.Abs(position.StartPrice - startPrice) < 1 && position.Quantity == quantity;
        }
    }
}
