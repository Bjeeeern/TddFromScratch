using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace Framework;

public static class TestRunner
{
    public static void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var testSuites = Assembly
            .GetCallingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && t.IsVisible && t.FullName!.Contains(nameof(TestSuites)));

        var exceptions = new List<XunitException>();
        foreach (var testSuite in testSuites)
        {
            var testMethods = testSuite
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == testSuite && m.Name != nameof(IDisposable.Dispose));
            foreach (var testMethod in testMethods)
            {
                try
                {
                    if (testMethod.IsStatic)
                    {
                        testMethod.Invoke(null, null);
                    }
                    else
                    {
                        var instance = testSuite.GetConstructors().Single().Invoke(null);
                        testMethod.Invoke(instance, null);

                        var disposeMethod = testSuite.GetMethod(nameof(IDisposable.Dispose));
                        disposeMethod?.Invoke(instance, null);
                    }
                }
                catch (TargetInvocationException outer)
                when (outer.InnerException is XunitException inner)
                {
                    exceptions.Add(inner);
                }
            }
        }

        foreach (var exception in exceptions)
        {
            var messageRows = exception.ToString().Split("\n");
            var slimmerMessage = string.Join(
                "\n",
                Enumerable
                    .Empty<string>()
                    .Append(messageRows.First())
                    .Concat(messageRows.Where(mr => mr.Contains(nameof(TestSuites)))));
            Console.WriteLine($"❌ {slimmerMessage}");
        }

        if (!exceptions.Any())
            Console.WriteLine("✅ All tests OK ✅");
    }
}
