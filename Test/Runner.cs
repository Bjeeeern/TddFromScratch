using System.Reflection;
using System.Text;
using Framework;

Console.OutputEncoding = Encoding.UTF8;

List<AssertException> exceptions = new();
IEnumerable<Type> testSuites = Assembly
.GetExecutingAssembly()
.GetTypes()
.Where(t => t.FullName!.StartsWith("TestSuites."));

foreach (var testSuite in testSuites)
{
    var testMethods = testSuite.GetMethods(BindingFlags.Static | BindingFlags.Public);
    foreach (var testMethod in testMethods)
    {
        try
        {
            testMethod.Invoke(null, null);
        }
        catch (TargetInvocationException outer)
        when (outer.InnerException is AssertException inner)
        {
            exceptions.Add(inner);
        }
    }
}

if (exceptions.Any())
{
    foreach (var exception in exceptions)
    {
        Console.WriteLine($"❌ {exception.TestMethod}:\n\t{exception.Message}\n\t-> {exception.TestMethodPath}");
    }
}
else
    Console.WriteLine("✅ All tests OK ✅");
