namespace Framework;

internal static class Assert
{
    public static void Empty<T>(IEnumerable<T>? enumeration)
    {
        if (enumeration?.Any() ?? true)
            throw new AssertException($"Expected to be empty but found {enumeration?.Count()} elements.");
    }

    public static void Single<T>(IEnumerable<T>? enumeration)
    {
        if (enumeration?.Count() != 1)
            throw new AssertException($"Expected single element but found {enumeration?.Count()} elements.");
    }
}