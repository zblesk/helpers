namespace zblesk.Helpers;

public static class StringExtensions
{
    static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string AddMarkdownQuote(this string text)
        => string.IsNullOrWhiteSpace(text)
            ? ""
            : "> " + text.Replace("\n", "\n> ");

    public static string RemoveDiacritics(this string text)
        => string.Concat(
            text.Normalize(NormalizationForm.FormD)
            .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
            .Normalize(NormalizationForm.FormC);

    public static string Repeat(this string text, int times)
    {
        if (times < 1)
            return "";
        var b = new StringBuilder();
        for (var i = 0; i < times; i++)
            b.Append(text);
        return b.ToString();
    }

    public static string NormalizeText(this string input)
        => input.RemoveDiacritics().Trim().ToLowerInvariant();

    public static string ReadableJoin(this IEnumerable<object> list)
        => list.Count() switch
        {
            0 => "",
            1 => $"{list.First()}",
            2 => $"{list.First()} a {list.Last()}",
            _ => string.Join(", ", list.Take(list.Count() - 1)) + $" a {list.Last()}"
        };

    public static string ReadableSizeSuffix(long value, int decimalPlaces = 2)
    {
        if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException(nameof(decimalPlaces)); }
        if (value < 0) { return "-" + ReadableSizeSuffix(-value); }
        if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        var mag = (int)Math.Log(value, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        var adjustedSize = (decimal)value / (1L << mag * 10);

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}",
            adjustedSize,
            SizeSuffixes[mag]);
    }

    public static string FormatRating(this int? rating)
        => rating switch
        {
            null => "",
            < 1 => "",
            < 5 => "⭐".Repeat(rating.Value),
            >= 5 => "🌟".Repeat(5),
        };

}
