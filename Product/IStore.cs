using Product;

internal interface IStore
{
    List<User> LoadUsers();

    void Store(List<User> users);
}