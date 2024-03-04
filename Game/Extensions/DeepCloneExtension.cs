using System.Reflection;

namespace Game.Extensions;

internal static class DeepCloneExtension
{
    internal static T DeepClone<T>(this T original) =>
        (T)original!.DeepClone(typeof(T));

    internal static object DeepClone(this object original, Type type)
    {
        var clone = type.GetConstructor(Array.Empty<Type>())!.Invoke(null);
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var prop in props)
        {
            var propValue = prop.GetValue(original);

            if (!IsPrimitive(prop.PropertyType))
                propValue = propValue?.DeepClone(prop.PropertyType);

            prop.SetValue(clone, propValue);
        }

        return clone;
    }

    private static bool IsPrimitive(Type type) =>
        type == typeof(string) ||
        type.IsValueType ||
        type.IsPrimitive;
}