namespace TestSuites;

public class StateServiceTests
{
    private const string PlayerAtXUnitVector = "player at X unit vector";
    private const string PlayerAtYUnitVector = "player at Y unit vector";
    private readonly StateService state;

    public StateServiceTests()
    {
        state = GameServices.Get()
            .BuildServiceProvider()
            .GetRequiredService<StateService>();
    }

    public void CanSetupScene()
    {
        Assert.Equal(Vector2.Zero, state.Current.Player.Position);

        state.AddScene(PlayerAtXUnitVector, GetSceneWithPlayerAt(Vector2.UnitX));
        state.LoadScene(PlayerAtXUnitVector);

        Assert.Equal(Vector2.UnitX, state.Current.Player.Position);
    }

    public void CanSwitchScenes()
    {
        state.AddScene(PlayerAtXUnitVector, GetSceneWithPlayerAt(Vector2.UnitX));
        state.AddScene(PlayerAtYUnitVector, GetSceneWithPlayerAt(Vector2.UnitY));

        state.LoadScene(PlayerAtXUnitVector);

        Assert.Equal(Vector2.UnitX, state.Current.Player.Position);

        state.LoadScene(PlayerAtYUnitVector);

        Assert.Equal(Vector2.UnitY, state.Current.Player.Position);
    }

    public void CanReloadScene()
    {
        state.AddScene(PlayerAtXUnitVector, GetSceneWithPlayerAt(Vector2.UnitX));
        state.LoadScene(PlayerAtXUnitVector);

        state.Current.Player.Position = Vector2.Zero;

        state.LoadScene(PlayerAtXUnitVector);

        Assert.Equal(Vector2.UnitX, state.Current.Player.Position);
    }

    private static SceneModel GetSceneWithPlayerAt(Vector2 vector) =>
        new() { Player = new() { Position = vector } };
}