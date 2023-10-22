#if !DEBUG

public static class HotReload
{
    public static int ReloadCount { get; private set; } = 0;
    public static event Action<Type[]?>? UpdateApplicationEvent;
}

#else

using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(Xero.HotReload))]

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

        types?.ToList().ForEach(type => Console.WriteLine($"Hot Reload (UpdateApplication): {type.FullName}"));

        UpdateApplicationEvent?.Invoke(types);
    }
}
#endif

internal class HotReloadContext<T> : IDisposable where T : IViewModel
{
    private UI<T>.Context context;
    public HotReloadContext(UI<T>.Context context)
    {
        this.context = context;
        HotReload.UpdateApplicationEvent += OnHotReload;
    }

    public void OnHotReload(Type[]? obj)
    {
        Task.Run(async () =>
        {
            await context.Recompose();
        });
    }

    public void Dispose()
    {
        HotReload.UpdateApplicationEvent -= OnHotReload;
    }
}