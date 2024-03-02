using System.Diagnostics;

namespace Framework;

internal class AssertException : Exception
{
    public string TestMethod { get; init; }

    public AssertException(string message)
    : base(message)
    {
        var stackTrace = new StackTrace();
        TestMethod = stackTrace.GetFrame(2)!.GetMethod()!.Name;
    }
}