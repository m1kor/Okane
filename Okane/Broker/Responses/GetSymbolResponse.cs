using Okane.Broker.Responses.Data;

namespace Okane.Broker.Responses
{
    public class GetSymbolResponse : Response
    {
        public Symbol returnData { get; set; }
    }
}
