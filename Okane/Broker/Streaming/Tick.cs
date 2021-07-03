using System;
using System.Text.Json.Serialization;

namespace Okane.Broker.Streaming
{
    [DataDescriptor("tickPrices")]
    public class Tick
    {
        // Ask price in base currency
        public decimal ask { get; set; }

        // Number of available lots to buy at given price or null if not applicable
        public int? askVolume { get; set; }

        // Bid price in base currency
        public decimal bid { get; set; }

        // Number of available lots to buy at given price or null if not applicable
        public int? bidVolume { get; set; }

        // The highest price of the day in base currency
        public decimal high { get; set; }

        // Price level
        public int level { get; set; }

        // The lowest price of the day in base currency
        public decimal low { get; set; }

        // Source of price, detailed description below
        public int quoteId { get; set; }

        // The difference between raw ask and bid prices
        public decimal spreadRaw { get; set; }

        // Spread representation
        public decimal spreadTable { get; set; }

        // Symbol
        public string symbol { get; set; }

        // Time
        [JsonIgnore]
        public DateTime time { get { return DateTimeExtensions.FromMsSince1970(timestamp); } set { timestamp = value.ToMsSince1970(); } }

        // Timestamp
        public long timestamp { get; set; }

        public Tick() { }

        public Tick(Tick t)
        {
            ask = t.ask;
            askVolume = t.askVolume;
            bid = t.bid;
            bidVolume = t.bidVolume;
            high = t.high;
            level = t.level;
            low = t.low;
            quoteId = t.quoteId;
            spreadRaw = t.spreadRaw;
            spreadTable = t.spreadTable;
            symbol = t.symbol;
            time = t.time;
            timestamp = t.timestamp;
        }
    }
}
