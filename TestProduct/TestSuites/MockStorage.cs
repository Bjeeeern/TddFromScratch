using Xunit;
using Product;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mocks;

namespace TestSuites;

public class MockStorage
{
    private readonly IStore store;

    public MockStorage()
    {
        store = ProductServices.Get()
            .Replace(ServiceDescriptor.Singleton<IStore, MockStore>())
            .BuildServiceProvider()
            .GetRequiredService<IStore>();
    }

    public void CanStoreAndLoadUsers()
    {
        var users = new List<User> { new User("test") };

        store.Store(users);
        var loadedUsers = store.LoadUsers();

        Assert.Equivalent(users, loadedUsers);
    }
}