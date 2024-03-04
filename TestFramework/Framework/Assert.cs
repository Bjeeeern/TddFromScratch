using Xunit.Sdk;

namespace Xunit;

public partial class Assert
{
    public static void GreaterThan<T>(T number, T actual)
        where T : IComparable<T>
    {
        if (number.CompareTo(actual) >= 0)
            throw new XunitException(
                $"Expected: {actual} > {number}{Environment.NewLine}{new string(' ', 10)}Assert.{nameof(GreaterThan)}() Failure");
    }
}
