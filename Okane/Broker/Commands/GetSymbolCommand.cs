using System.Collections.Generic;

namespace Okane.Broker.Commands
{
    class GetSymbolCommand : Command
    {
        public override string command { get; set; } = "getSymbol";

        public Dictionary<string, string> arguments { get; set; }

        public GetSymbolCommand(string symbol)
        {
            arguments = new Dictionary<string, string> { { "symbol", symbol } };
        }
    }
}
