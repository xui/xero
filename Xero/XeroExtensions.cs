using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.AspNetCore.Routing;

namespace Xero;

public static class XeroExtensions
{
    public static void AddXero(this IServiceCollection services)
    {
        services.AddWebSockets(options =>
        {
        });
    }

    public static WebApplication MapUI<T>(
        this WebApplication app,
        [StringSyntax("Route")] string pattern,
        UI<T> ui) where T : IViewModel
    {
        app.UseWebSockets();

        var group = app.MapGroup(pattern);

        group.MapGet("/", async httpContext =>
        {
            var xeroContext = UI<T>.XeroMemoryCache.Get(httpContext);

            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                // TODO: This is almost correct.  Work across multiple browsers but multiple tabs gets its Action stolen.
                // Rework this once you figure out the various ViewModel state levels.
                xeroContext.ViewModel.OnChanged = () => ui.Recompose(xeroContext);
#if DEBUG
                // TODO: Move this to Receive so that when the websocket closes it can -= itself?
                HotReload.UpdateApplicationEvent += types => ui.Recompose(xeroContext);
#endif

                await xeroContext.AssignWebSocket(httpContext.WebSockets);
            }
            else
            {
                // TODO: Optimize.  No need to convert to a single string when we 
                // have streams and pipes.
                await httpContext.Response.WriteAsync(ui.Compose(xeroContext).ToStringWithExtras());
            }
        });

        ui.MapPages();

        return app;
    }
}
