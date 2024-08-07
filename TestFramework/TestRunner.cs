﻿using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace Framework;

public static class TestRunner
{
    public static async Task Run(Assembly assembly)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var testSuites = assembly
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
                    var instance = testMethod.IsStatic
                        ? null
                        : testSuite.GetConstructors().Single().Invoke(null);

                    if (testMethod.Invoke(instance, null) is Task task)
                    {
                        await task;
                    }

                    if (instance != null &&
                        testSuite.GetMethod(nameof(IDisposable.Dispose)) is MethodInfo disposer)
                    {
                        disposer.Invoke(instance, null);
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
