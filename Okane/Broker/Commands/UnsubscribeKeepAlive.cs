namespace Okane.Broker.Commands
{
    public class UnsubscribeKeepAlive : StreamingCommand
    {
        public override string command { get; set; } = "stopKeepAlive";
    }
}
