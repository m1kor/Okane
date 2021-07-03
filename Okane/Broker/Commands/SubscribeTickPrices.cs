namespace Okane.Broker.Commands
{
    public class SubscribeTickPrices : StreamingCommand
    {
        public override string command { get; set; } = "getTickPrices";

        public string streamSessionId { get; set; }

        public string symbol { get; set; }

        public int? minArrivalTime { get; set; }

        public int? maxLevel { get; set; }

        public SubscribeTickPrices(string streamSessionId, string symbol, int? minArrivalTime = null, int? maxLevel = null)
        {
            this.streamSessionId = streamSessionId;
            this.symbol = symbol;
            this.minArrivalTime = minArrivalTime;
            this.maxLevel = maxLevel;
        }
    }
}
