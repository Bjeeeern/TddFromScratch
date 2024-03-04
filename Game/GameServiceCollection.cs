using Game.Services;

namespace Game;

public static class GameServiceCollection
{
    public static IServiceProvider Initialize()
    {
        var services = new ServiceCollection()
            .AddSingleton<GraphicsDeviceManager>()
            .AddSingleton<XnaGame, XnaGameWrapperService>()
            .AddSingleton(provider =>
            {
                provider.GetRequiredService<GraphicsDeviceManager>();
                return (XnaGameWrapperService)provider.GetRequiredService<XnaGame>();
            })
            .AddSingleton<StateService>()
            .AddSingleton<GameService>();

        return services.BuildServiceProvider();
    }
}