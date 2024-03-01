namespace Product;

internal class UserRegister
{
    private readonly List<int> users = new List<int>();
    internal IReadOnlyCollection<int> Users => users;
    internal void RegisterUser(string name, string password)
    {
        users.Add(0);
    }
}