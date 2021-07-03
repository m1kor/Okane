namespace Okane.Broker.Commands
{
    public class SubscribeProfits : StreamingCommand
    {
        public override string command { get; set; } = "getProfits";

        public string streamSessionId { get; set; }

        public SubscribeProfits(string streamSessionId)
        {
            this.streamSessionId = streamSessionId;
        }
    }
}
