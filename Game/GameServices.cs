using Game.Services;

namespace Game;

public static class GameServices
{
    public static IServiceCollection Get() =>
        new ServiceCollection()
            .AddSingleton<GraphicsDeviceManager>()
            .AddSingleton<XnaGame, XnaGameWrapperService>()
            .AddSingleton(provider =>
            {
                provider.GetRequiredService<GraphicsDeviceManager>();
                return (XnaGameWrapperService)provider.GetRequiredService<XnaGame>();
            })
            .AddSingleton<StateService>()
            .AddSingleton<GameService>();
}