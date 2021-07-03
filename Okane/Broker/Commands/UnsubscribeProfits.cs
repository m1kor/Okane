namespace Okane.Broker.Commands
{
    public class UnsubscribeProfits : StreamingCommand
    {
        public override string command { get; set; } = "stopProfits";
    }
}
