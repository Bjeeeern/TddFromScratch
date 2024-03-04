using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace TestSuites;

public class GameIntegrationTests : IDisposable
{
    private readonly IServiceProvider provider;
    private readonly GameService game;
    private readonly StateService state;

    public GameIntegrationTests()
    {
        provider = GameServiceCollection.Initialize();
        game = provider.GetRequiredService<GameService>();
        state = provider.GetRequiredService<StateService>();

        state.Current.Player.SpriteAsset = "Villager";

        typeof(GraphicsDevice).Assembly
            .GetType("Microsoft.Xna.Framework.Threading")!
            .GetField("_mainThreadId", BindingFlags.Static | BindingFlags.NonPublic)!
            .SetValue(null, Thread.CurrentThread.ManagedThreadId);
    }

    public void Dispose()
    {
        var wrapper = provider.GetRequiredService<XnaGameWrapperService>();
        wrapper.Dispose();
    }

    public void GraphicsDeviceManagerInstantiatedWhenGameInstantiated()
    {
        // Microsoft.Xna.Framework.GraphicsDeviceManager (GDM) has a dependency on Microsoft.Xna.Framework.Game (XnaGame)
        // but XnaGame also has a implicit runtime dependency on GDM, so GDM needs to be instantiated whenever XnaGame is
        // instantiated if it hasn't already been.
        game.RunOneFrame();
    }

    public void AllHandlersInitialize()
    {
        var wrapper = provider.GetRequiredService<XnaGameWrapperService>();
        var handlers = typeof(XnaGameWrapperService)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name.EndsWith("Handler"));

        foreach (var handler in handlers)
        {
            Assert.NotNull(handler.GetValue(wrapper));
        }
    }
    public void CanMovePlayerDown()
    {
        state.Current.ControllerDirection = Vector2.UnitX;
        game.RunOneFrame();

        Assert.GreaterThan(0, state.Current.Player.Position.X);
    }

    public void CanRenderPlayer()
    {
        var gdm = provider.GetRequiredService<GraphicsDeviceManager>();
        var wrapper = provider.GetRequiredService<XnaGameWrapperService>();

        gdm.ApplyChanges();

        var image = new Color[32 * 32];
        var render = new Color[32 * 32];

        Texture2D.FromStream(gdm.GraphicsDevice, File.OpenRead(@"..\Game\Assets\Intermediate\Villager.png"))
            .GetData(image);

        wrapper.DrawHandler += () =>
        {
            var backBufferWidth = gdm.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = gdm.GraphicsDevice.PresentationParameters.BackBufferHeight;
            var expectedArea = new Rectangle(
                backBufferWidth / 2 - 32 / 2,
                backBufferHeight / 2 - 32 / 2,
                32,
                32);

            gdm.GraphicsDevice.GetBackBufferData(expectedArea, render, 0, 32 * 32);
        };

        game.RunOneFrame();

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                var expected = image[y * 32 + x];
                var actual = render[y * 32 + x];

                actual.A = expected.A;
                Assert.Equal(expected, actual);
            }
        }
    }
}