using System.Text.Json.Serialization;

namespace Okane.Broker.Responses.Data
{
    // SYMBOL_RECORD
    public class Symbol
    {
        // 	Ask price in base currency
        public float ask { get; set; }

        // Bid price in base currency
        public float bid { get; set; }

        // Category name
        public string categoryName { get; set; }

        // Size of 1 lot
        public int contractSize { get; set; }

        // Currency
        public string currency { get; set; }

        // Indicates whether the symbol represents a currency pair
        public bool currencyPair { get; set; }

        // The currency of calculated profit
        public string currencyProfit { get; set; }

        // Description
        public string description { get; set; }

        // Null if not applicable
        [JsonPropertyName("expiration")]
        public long? expirationMs { get; set; }

        // Symbol group name
        public string groupName { get; set; }

        // The highest price of the day in base currency
        public float high { get; set; }

        // Initial margin for 1 lot order, used for profit/margin calculation
        public int initialMargin { get; set; }

        // Maximum instant volume multiplied by 100 (in lots)
        public int instantMaxVolume { get; set; }

        // Symbol leverage
        public float leverage { get; set; }

        // Long only
        public bool longOnly { get; set; }

        // Maximum size of trade
        public float lotMax { get; set; }

        // Minimum size of trade
        public float lotMin { get; set; }

        // A value of minimum step by which the size of trade can be changed (within lotMin - lotMax range)
        public float lotStep { get; set; }

        // The lowest price of the day in base currency
        public float low { get; set; }

        // Used for profit calculation
        public int marginHedged { get; set; }

        // For margin calculation
        public bool marginHedgedStrong { get; set; }

        // For margin calculation, null if not applicable
        public int? marginMaintenance { get; set; }

        // For margin calculation
        public int marginMode { get; set; }

        // Percentage
        public float percentage { get; set; }

        // Number of symbol's pip decimal places
        public int pipsPrecision { get; set; }

        // Number of symbol's price decimal places
        public int precision { get; set; }

        // For profit calculation
        public int profitMode { get; set; }

        // Source of price
        public int quoteId { get; set; }

        // 	Indicates whether short selling is allowed on the instrument
        public bool shortSelling { get; set; }

        // The difference between raw ask and bid prices
        public float spreadRaw { get; set; }

        // Spread representation
        public float spreadTable { get; set; }

        // Null if not applicable
        [JsonPropertyName("starting")]
        public long? startingMs { get; set; }

        // Appropriate step rule ID from getStepRules command response
        public int stepRuleId { get; set; }

        // Minimal distance (in pips) from the current price where the stopLoss/takeProfit can be set
        public int stopsLevel { get; set; }

        // Time when additional swap is accounted for weekend
        public int swap_rollover3days { get; set; }

        // Indicates whether swap value is added to position on end of day
        public bool swapEnable { get; set; }

        // Swap value for long positions in pips
        public float swapLong { get; set; }

        // Swap value for short positions in pips
        public float swapShort { get; set; }

        // Type of swap calculated
        public int swapType { get; set; }

        // Symbol name
        public string symbol { get; set; }

        // Smallest possible price change, used for profit/margin calculation, null if not applicable
        public float tickSize { get; set; }

        // Value of smallest possible price change (in base currency), used for profit/margin calculation, null if not applicable
        public float tickValue { get; set; }

        // Ask & bid tick time
        [JsonPropertyName("time")]
        public long timeMs { get; set; }

        // Time in String
        public string timeString { get; set; }

        // Indicates whether trailing stop (offset) is applicable to the instrument.
        public bool trailingEnabled { get; set; }

        // Instrument class number
        public int type { get; set; }
    }
}
