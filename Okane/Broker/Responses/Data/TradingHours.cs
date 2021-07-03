using System.Collections.Generic;
using System.Linq;

namespace Okane.Broker.Responses.Data
{
    public class TradingHours
    {
        public List<Quote> quotes { get; set; }

        public string symbol { get; set; }

        public List<Trading> trading { get; set; }

        // TODO
        public bool IsOpen()
        {
            var now = Client.Now();
            var dayOfWeek = (int)now.DayOfWeek;
            int dayOfWeekInt = (dayOfWeek == 0) ? 7 : dayOfWeek;
            var day = trading.FirstOrDefault(x => x.day == dayOfWeekInt);
            if (day.fromT == day.toT)
            {
                return true;
            }
            else
            {
                return day != null ? now >= day.From && now <= day.To : false;
            }
        }
    }
}
