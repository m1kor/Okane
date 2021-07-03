using System.Collections.Generic;

namespace Okane.Broker.Responses
{
    public class GetMarginTradeResponse : Response
    {
        public Dictionary<string, decimal> returnData { get; set; }
    }
}
