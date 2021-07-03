using System.Text.Json.Serialization;

namespace Okane.Broker.Responses.Data
{
    public class ServerTime
    {
        [JsonPropertyName("time")]
        public long timeMs { get; set; }

        public string timeString { get; set; }
    }
}
