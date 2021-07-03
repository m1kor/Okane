using Okane.Broker.Responses.Data;

namespace Okane.Broker.Responses
{
    public class GetServerTimeResponse : Response
    {
        public ServerTime returnData { get; set; }
    }
}
