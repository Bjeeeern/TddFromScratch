using Game.Extensions;

namespace Game.Services;

public class StateService
{
    private readonly Dictionary<string, SceneModel> scenes = new();

    internal SceneModel Current { get; private set; } = new SceneModel();

    public StateService()
    {
        AddScene("default", Current); 
    }

    internal void AddScene(string sceneName, SceneModel scene)
    {
        scenes.Add(sceneName, scene);
    }

    internal void LoadScene(string sceneName)
    {
        Current = scenes[sceneName].DeepClone();
    }
}