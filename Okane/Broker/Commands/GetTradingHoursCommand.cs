using System.Collections.Generic;

namespace Okane.Broker.Commands
{
    public class GetTradingHoursCommand : Command
    {
        public override string command { get; set; } = "getTradingHours";
        public Dictionary<string, List<string>> arguments { get; set; }

        public GetTradingHoursCommand(List<string> symbols)
        {
            arguments = new Dictionary<string, List<string>>();
            arguments["symbols"] = new List<string>(symbols);
        }
    }
}
