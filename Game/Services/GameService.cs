using Microsoft.Xna.Framework.Graphics;

namespace Game.Services;

public class GameService
{
    private Matrix worldMatrix;
    private readonly StateService state;
    private readonly XnaGameWrapperService wrapper;
    private SpriteBatch? spriteBatch;
    private readonly Dictionary<string, Texture2D> spriteCache = new();

    public GameService(StateService state, XnaGameWrapperService wrapper)
    {
        this.state = state;
        this.wrapper = wrapper;

        wrapper.LoadContentHandler = LoadContent;
        wrapper.UpdateHandler = Update;
        wrapper.DrawHandler = Draw;

        wrapper.Content.RootDirectory = "Content";
    }

    public void Run() =>
        wrapper.Run();

    internal void RunOneFrame() =>
        wrapper.RunOneFrame();

    private void LoadContent()
    {
        spriteBatch = new SpriteBatch(wrapper.GraphicsDevice);

        var texture = wrapper.Content.Load<Texture2D>(state.Current.Player.SpriteAsset);
        spriteCache.Add(state.Current.Player.SpriteAsset!, texture);

        worldMatrix = Matrix.CreateTranslation(wrapper.GraphicsDevice.Viewport.Width / 2, wrapper.GraphicsDevice.Viewport.Height / 2, 0);
    }

    private void Update()
    {
        state.Current.Player.Position = state.Current.ControllerDirection;
    }

    private void Draw()
    {
        spriteBatch!.Begin(transformMatrix: worldMatrix);

        var entity = state.Current.Player;

        var position = entity.Position.ToPoint();
        var sprite = spriteCache[entity.SpriteAsset!]!;

        spriteBatch.Draw(sprite, new Rectangle(position.X - sprite.Width / 2, position.Y - sprite.Width / 2, sprite.Width, sprite.Height), Color.White);
        spriteBatch.End();
    }
}