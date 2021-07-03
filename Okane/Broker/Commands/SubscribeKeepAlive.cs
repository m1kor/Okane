namespace Okane.Broker.Commands
{
    public class SubscribeKeepAlive : StreamingCommand
    {
        public override string command { get; set; } = "getKeepAlive";

        public string streamSessionId { get; set; }

        public SubscribeKeepAlive(string streamSessionId)
        {
            this.streamSessionId = streamSessionId;
        }
    }
}
