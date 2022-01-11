namespace zblesk.Helpers;

public static class Extensions
{
    /// <summary>
    /// Format an exception for printing.
    /// </summary>
    /// <param name="ex">The exception to print.</param>
    public static string FormatException(this Exception ex) 
        => $"{ex.Message}\n{ex.StackTrace}{(ex.InnerException == null ? "" : "\nInner: " + ex.InnerException)}";

    public static int Bound(this int number, int min, int max)
        => Math.Min(max, Math.Max(min, number));
}