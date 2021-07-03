using Okane.Broker.Responses.Data;
using System.Collections.Generic;

namespace Okane.Broker.Responses
{
    public class GetTradingHoursResponse : Response
    {
        public List<TradingHours> returnData { get; set; }
    }
}
