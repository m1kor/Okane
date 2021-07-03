namespace Okane.Broker.Streaming
{
    [DataDescriptor("profit")]
    public class Profit
    {
        // Order number
        public int order { get; set; }

        // Transaction ID
        public int order2 { get; set; }

        // Position number
        public int position { get; set; }

        // Profit in account currency
        public decimal profit { get; set; }
    }
}
