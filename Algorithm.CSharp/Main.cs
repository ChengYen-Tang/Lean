using QuantConnect.Brokerages;
using QuantConnect.Data;
using System.Linq;

namespace QuantConnect.Algorithm.CSharp
{
    public class BinanceCryptoFutureDataAlgorithm : QCAlgorithm
    {
        public Symbol _symbol;

        public override void Initialize()
        {
            SetStartDate(2022, 10, 1);
            SetEndDate(2022, 10, 10);
            SetCash("BUSD", 100000);

            SetBrokerageModel(BrokerageName.BinanceFutures, AccountType.Margin);

            var cryptoFuture = AddCryptoFuture("BTCBUSD", Resolution.Minute);
            // perpetual futures does not have a filter function
            _symbol = cryptoFuture.Symbol;

            // Historical data
            var history = History(_symbol, 10, Resolution.Minute);
            Debug($"We got {history.Count()} from our history request for {_symbol}");
        }

        public override void OnData(Slice slice)
        {
            if (!slice.Bars.ContainsKey(_symbol) || !slice.QuoteBars.ContainsKey(_symbol))
            {
                return;
            }

            var quote = slice.QuoteBars[_symbol];
            var price = slice.Bars[_symbol].Price;

            if (price - quote.Bid.Close > quote.Ask.Close - price)
            {
                SetHoldings(_symbol, -1m);
            }
            else
            {
                SetHoldings(_symbol, 1m);
            }
        }
    }
}
