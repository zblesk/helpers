namespace zblesk.Helpers;

/// <summary>
/// An invariant-culture string comparer that ignores diacritics.
/// </summary>
public class IgnoreCaseDiacritics : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y) 
        => string.Compare(
            x,
            y,
            CultureInfo.InvariantCulture,
            CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;

    public int GetHashCode(string obj) 
        => obj
            .RemoveDiacritics()
            .ToUpper()
            .GetHashCode();
}
