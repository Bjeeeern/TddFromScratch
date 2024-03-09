using Xunit.Sdk;

namespace Xunit;

public partial class Assert
{
    public static void GreaterThan<T>(T number, T actual)
        where T : IComparable<T>
    {
        if (number.CompareTo(actual) >= 0)
            throw new XunitException(
                $"Expected {actual} > {number}");
    }

    public static void DirectoryExists(string expectedPath)
    {
        if (!Directory.Exists(expectedPath))
            throw new XunitException(
                $"Expected {expectedPath} to exist");
    }
}
