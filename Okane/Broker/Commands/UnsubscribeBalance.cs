namespace Okane.Broker.Commands
{
    public class UnsubscribeBalance : StreamingCommand
    {
        public override string command { get; set; } = "stopBalance";
    }
}
