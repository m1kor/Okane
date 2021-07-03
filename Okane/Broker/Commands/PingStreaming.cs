namespace Okane.Broker.Commands
{
    public class PingStreaming : StreamingCommand
    {
        public override string command { get; set; } = "ping";

        public string streamSessionId { get; set; }

        public PingStreaming(string streamSessionId)
        {
            this.streamSessionId = streamSessionId;
        }
    }
}
