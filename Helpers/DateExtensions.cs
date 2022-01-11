namespace zblesk.Helpers;

public static class DateExtensions
{
    public static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime FromUnixTimestamp(this long ticks) => Epoch.AddSeconds(ticks);

    public static int ToUnixTimestamp(this DateTime date) => (int)date.Subtract(Epoch).TotalSeconds;

    public static long ToUnixTimestampMiliseconds(this DateTime date) => (long)date.Subtract(Epoch).TotalMilliseconds;

}
