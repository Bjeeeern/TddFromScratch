using Framework;
using Product;

[TestSuite]
internal static class UserRegisterTests
{
    public static void NoUsersExistsInitially()
    {
        var userRegister = CleanSetup();

        Assert.Empty(userRegister.Users);
    }

    public static void CanRegisterUser()
    {
        var userRegister = CleanSetup();
        userRegister.RegisterUser("test.testsson", "qwerty123456");

        Assert.Single(userRegister.Users);
    }

    public static void RegisteredUserPersists()
    {
        var userRegister = CleanSetup();
        userRegister.RegisterUser("test.testsson", "qwerty123456");

        // reload app
        userRegister = new UserRegister("tmp");

        Assert.Single(userRegister.Users);
    }

    private static UserRegister CleanSetup()
    {
        if (Directory.Exists("tmp"))
            Directory.Delete("tmp", recursive: true);

        return new UserRegister("tmp");
    }
}