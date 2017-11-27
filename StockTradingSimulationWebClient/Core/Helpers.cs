using StockTradingSimulationWebClient.Models;
using System;

namespace StockTradingSimulationWebClient.Core
{
    public static class Helpers
    {
        public static float CalculatePositionEquity(Position position, string token)
        {
            float price;
            if (position.CloseDatetime <= position.OpenDatetime)
                price = ApiClient.GetStockPrice(token, position.StockId);
            else
                price = position.StartPrice;
            if (position.TransactionType == Transaction.BUY)
                return position.Quantity * price;
            if (position.TransactionType == Transaction.SELL_SHORT)
                return (position.StartPrice * 2 - price) * 100;
            throw new Exception("Unsupported transaction type.");
        }
    }
}