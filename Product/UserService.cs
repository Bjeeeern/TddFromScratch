namespace Product;

internal class UserService
{
    private readonly List<User> users = new();
    private readonly IStore store;

    public UserService(IStore store)
    {
        this.store = store;
        users = store.LoadUsers();
    }

    internal IReadOnlyCollection<User> Users => users;

    internal void RegisterUser(string name, string password)
    {
        users.Add(new User(name));
        store.Store(users);
    }
}