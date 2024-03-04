namespace Game.Services;

public class XnaGameWrapperService : XnaGame
{
    public Action? LoadContentHandler { get; internal set; }
    public Action? UpdateHandler { get; internal set; }
    public Action? DrawHandler { get; internal set; }

    protected override void LoadContent() =>
        LoadContentHandler!.Invoke();

    protected override void Update(GameTime gameTime) =>
        UpdateHandler!.Invoke();

    protected override void Draw(GameTime gameTime) =>
        DrawHandler!.Invoke();
}