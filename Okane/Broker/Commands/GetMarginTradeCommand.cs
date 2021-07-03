namespace Okane.Broker.Commands
{
    public class GetMarginTradeCommand : Command
    {
        public override string command { get; set; } = "getMarginTrade";

        public struct Arguments
        {
            public string symbol { get; set; }

            public decimal volume { get; set; }
        }

        public Arguments arguments { get; set; }

        public GetMarginTradeCommand(string symbol, decimal volume)
        {
            arguments = new Arguments() { symbol = symbol, volume = volume };
        }
    }
}
