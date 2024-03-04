namespace Game.Models;

internal class SceneModel
{
    internal Vector2 ControllerDirection { get; set; }

    internal EntityModel Player { get; set; } = new EntityModel();

    internal class EntityModel
    {
        internal string? SpriteAsset { get; set; }

        internal Vector2 Position { get; set; }
    }
}
