#if !DEBUG
public static class HotReload
{
    public static int ReloadCount { get; private set; } = 0;
}
#else
using System.Text.Json;

[assembly: System.Reflection.Metadata.MetadataUpdateHandlerAttribute(typeof(Xero.HotReload))]

namespace Xero;

public static class HotReload
{
    public static int ReloadCount { get; private set; } = 0;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public static event Action<Type[]?>? UpdateApplicationEvent;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    internal static void ClearCache(Type[]? types)
    {
        types?.ToList().ForEach(type => Console.WriteLine($"Hot Reload (ClearCache): {type.FullName}"));
    }

    internal static void UpdateApplication(Type[]? types)
    {
        ReloadCount++;

        types?.ToList().ForEach(type => Console.WriteLine($"Hot Reload (UpdateApplication): {types[0].Name}"));

        UpdateApplicationEvent?.Invoke(types);
    }
}
#endif