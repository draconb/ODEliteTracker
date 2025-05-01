namespace ODEliteTracker.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime LatestTime(this IEnumerable<DateTime> dateTimes)
        {
            return dateTimes.Order().LastOrDefault();
        }
    }
}
