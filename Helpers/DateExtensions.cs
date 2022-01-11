namespace zblesk.Helpers;

public static class DateExtensions
{
    /// <summary>
    /// The start of the Unix epoch.
    /// </summary>
    public static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Unix timestamp to DateTime.
    /// </summary>
    /// <param name="ticks">Unix timestamp.</param>
    /// <returns></returns>
    public static DateTime FromUnixTimestamp(this long ticks) => Epoch.AddSeconds(ticks);

    /// <summary>
    /// From DateTime to Unix Timestamp in seconds.
    /// </summary>
    public static int ToUnixTimestamp(this DateTime date) => (int)date.Subtract(Epoch).TotalSeconds;

    /// <summary>
    /// From DateTime to Unix Timestamp in miliseconds.
    /// </summary>
    public static long ToUnixTimestampMiliseconds(this DateTime date) => (long)date.Subtract(Epoch).TotalMilliseconds;

}
