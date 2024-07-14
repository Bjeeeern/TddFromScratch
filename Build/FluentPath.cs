using System.Reflection;

namespace Build;

public class FluentPath
{
    private string path;

    private FluentPath(string path)
    {
        this.path = path;
    }

    public static FluentPath From(string path)
    {
        return new FluentPath(path);
    }

    public FluentPath Combine(params string[] paths)
    {
        path = Path.Combine(paths.Prepend(path).ToArray());
        return this;
    }

    public FluentPath GetDirectoryName()
    {
        path = Path.GetDirectoryName(path) ?? Path.GetPathRoot(path) ?? string.Empty;
        return this;
    }

    public FluentPath GetFileName()
    {
        path = Path.GetFileName(path);
        return this;
    }

    public FluentPath GetFullPath()
    {
        path = Path.GetFullPath(path);
        return this;
    }

    public string Build()
    {
        return path;
    }
}