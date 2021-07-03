namespace Okane.Broker.Streaming
{
    [DataDescriptor("balance")]
    public class Balance
    {
        // Balance in account currency
        public float balance { get; set; }

        // Credit in account currency
        public float credit { get; set; }

        // Sum of balance and all profits in account currency
        public float equity { get; set; }

        // Margin requirements
        public float margin { get; set; }

        // Free margin
        public float marginFree { get; set; }

        // Margin level percentage
        public float marginLevel { get; set; }

        public float stockValue { get; set; }

        public float stockLock { get; set; }

        public float cashStockValue { get; set; }
    }
}
