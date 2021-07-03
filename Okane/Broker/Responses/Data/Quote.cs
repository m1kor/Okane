using System;
using System.Text.Json.Serialization;

namespace Okane.Broker.Responses.Data
{
    public class Quote
    {
        // Day of week
        public int day { get; set; }

        [JsonIgnore]
        public DateTime From { get; set; }

        // Start time in ms from 00:00 CET / CEST time zone (see Daylight Saving Time, DST)
        public int fromT { get { return From.ToMsFromMidnight(); } set { From = DateTime.Today.AddMilliseconds(value); } }

        [JsonIgnore]
        public DateTime To { get; set; }

        // End time in ms from 00:00 CET / CEST time zone (see Daylight Saving Time, DST)
        public int toT { get { return To.ToMsFromMidnight(); } set { To = DateTime.Today.AddMilliseconds(value); } }
    }
}
