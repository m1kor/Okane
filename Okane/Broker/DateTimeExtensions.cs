using System;

namespace Okane.Broker
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int ToMsFromMidnight(this DateTime time)
        {
            return (((time.Hour * 60) + time.Minute) * 60 + time.Second) * 1000 + time.Millisecond;
        }

        public static DateTime FromMsSince1970(long msSince1970)
        {
            return TimeZoneInfo.ConvertTime(epoch.AddMilliseconds(msSince1970), TimeZoneInfo.Utc, Client.TimeZone);
        }

        public static long ToMsSince1970(this DateTime time)
        {
            return (long)(time - epoch).TotalMilliseconds;
        }
    }
}
