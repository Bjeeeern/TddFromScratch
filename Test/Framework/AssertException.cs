using System.Diagnostics;

namespace Framework;

internal class AssertException : Exception
{
    public string TestMethod { get; init; }

    public string TestMethodPath { get; init; }

    public AssertException(string message)
    : base(message)
    {
        var frame = new StackTrace(fNeedFileInfo: true).GetFrame(2)!;
        TestMethod = frame.GetMethod()!.Name;
        TestMethodPath = $"{frame.GetFileName()}:{frame.GetFileLineNumber()}:{frame.GetFileColumnNumber()}";
    }
}