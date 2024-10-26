namespace Branch.API.Extensions
{
    public static partial class DateTimeExtension
    {
        public static DateTime SetTime(this DateTime current, int hour, int minute)
        {
            return SetTime(current, hour, minute, 0, 0);
        }
        public static DateTime SetTime(this DateTime current, int hour, int minute, int second, int millisecond)
        {
            return new DateTime(current.Year, current.Month, current.Day, hour, minute, second, millisecond, DateTimeKind.Utc);
        }
    }
}
