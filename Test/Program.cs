using System.Text;
using Product;

Console.OutputEncoding = Encoding.UTF8;

CanRegisterUser();

Console.WriteLine("✅ All tests OK ✅");

static void CanRegisterUser()
{
    var userRegister = new UserRegister();

    AssertEmpty(userRegister.Users);

    userRegister.RegisterUser("test.testsson", "qwerty123456");

    AssertSingle(userRegister.Users);
}

static void AssertEmpty<T>(IEnumerable<T>? enumeration)
{
    if (enumeration?.Any() ?? true)
        throw new Exception($"Expected to be empty but found {enumeration?.Count()} elements.");
}

static void AssertSingle<T>(IEnumerable<T>? enumeration)
{
    if (enumeration?.Count() != 1)
        throw new Exception($"Expected single element but found {enumeration?.Count()} elements.");
}