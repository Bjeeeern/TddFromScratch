using Product;

namespace Mocks;

internal class MockStore : IStore
{
    private List<User> users = new();

    public List<User> LoadUsers() => users;

    public void Store(List<User> users) => this.users = users;
}