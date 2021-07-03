namespace Okane.Broker.Commands
{
    public class SubscribeBalance : StreamingCommand
    {
        public override string command { get; set; } = "getBalance";

        public string streamSessionId { get; set; }

        public SubscribeBalance(string streamSessionId)
        {
            this.streamSessionId = streamSessionId;
        }
    }
}
