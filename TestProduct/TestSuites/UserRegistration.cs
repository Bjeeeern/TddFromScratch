using Xunit;
using Product;
using Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TestSuites;

public class UserRegistration
{
    private readonly IServiceCollection services;
    private readonly UserService userRegister;
    private readonly MockStore mockStore = new();

    public UserRegistration()
    {
        services = ProductServices.Get()
            .Replace(ServiceDescriptor.Singleton<IStore>(mockStore));
        userRegister = services
            .BuildServiceProvider()
            .GetRequiredService<UserService>();
    }

    public void NoUsersExistsInitially()
    {
        Assert.Empty(userRegister.Users);
    }

    public void CanRegisterUser()
    {
        userRegister.RegisterUser("test.testsson", "qwerty123456");

        var user = Assert.Single(userRegister.Users);
        Assert.Equal("test.testsson", user.Name);
    }

    public void RegisteredUserPersists()
    {
        userRegister.RegisterUser("test.testsson", "qwerty123456");

        var reloadedUserRegister = services
            .BuildServiceProvider()
            .GetRequiredService<UserService>();

        Assert.Single(reloadedUserRegister.Users);
        Assert.Equivalent(userRegister.Users, reloadedUserRegister.Users);
    }
}