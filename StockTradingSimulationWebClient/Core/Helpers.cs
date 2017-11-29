using StockTradingSimulationWebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockTradingSimulationWebClient.Core
{
    public static class Helpers
    {
        public static float CalculatePositionEquity(Position position, float cPrice)
        {
            float price;
            if (position.CloseDatetime <= position.OpenDatetime)
                price = cPrice;
            else
                price = position.StartPrice;
            if (position.TransactionType == Transaction.BUY)
                return position.Quantity * price;
            if (position.TransactionType == Transaction.SELL_SHORT)
                return position.Quantity * (2 * position.StartPrice - price);
            throw new Exception("Unsupported transaction type.");
        }

        public static IEnumerable<StockPrice> GetStockPrices(string token)
        {
            var stocks = ApiClient.GetStocks(token);
            var stockPrices = stocks.Select(s => new StockPrice
            {
                StockId = s.Id,
                Ticker = s.Ticker,
                Price = ApiClient.GetStockPrice(token, s.Id)
            });

            return stockPrices;
        }
    }
}