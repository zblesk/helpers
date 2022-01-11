using System.Globalization;

namespace zblesk.Helpers;

public class IgnoreCaseDiacritics : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        return string.Compare(
            x,
            y,
            CultureInfo.InvariantCulture,
            CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;
    }

    public int GetHashCode(string obj)
    {
        return obj
            .RemoveDiacritics()
            .ToUpper()
            .GetHashCode();
    }
}
