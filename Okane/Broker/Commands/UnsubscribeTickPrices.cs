namespace Okane.Broker.Commands
{
    class UnsubscribeTickPrices : StreamingCommand
    {
        public override string command { get; set; } = "stopTickPrices";

        public string symbol { get; set; }

        public UnsubscribeTickPrices(string symbol)
        {
            this.symbol = symbol;
        }
    }
}
