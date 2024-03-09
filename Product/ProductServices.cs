using Microsoft.Extensions.DependencyInjection;

namespace Product;

public static class ProductServices
{
    public static IServiceCollection Get() =>
        new ServiceCollection()
            .AddSingleton<UserService>();
}