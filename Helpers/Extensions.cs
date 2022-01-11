namespace zblesk.Helpers;

public static class Extensions
{
    /// <summary>
    /// Format an exception for printing.
    /// </summary>
    /// <param name="ex">The exception to print.</param>
    public static string FormatException(this Exception ex) 
        => $"{ex.Message}\n{ex.StackTrace}{(ex.InnerException == null ? "" : "\nInner: " + ex.InnerException)}";

    /// <summary>
    /// Fits a number between bounds. Sets it to Min or Max if it's exceeded.
    /// </summary>
    /// <param name="number">Number to clamp.</param>
    /// <param name="min">Min value.</param>
    /// <param name="max">Max value.</param>
    /// <returns>Min, max, or number if min <= number <= max.</returns>
    public static int Clamp(this int number, int min, int max)
        => Math.Min(max, Math.Max(min, number));
}