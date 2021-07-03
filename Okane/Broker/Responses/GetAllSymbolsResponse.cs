using Okane.Broker.Responses.Data;
using System.Collections.Generic;

namespace Okane.Broker.Responses
{
    public class GetAllSymbolsResponse : Response
    {
        public List<Symbol> returnData { get; set; }
    }
}
