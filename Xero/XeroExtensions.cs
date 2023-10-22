using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.AspNetCore.Routing;
using System.Net.WebSockets;

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
        app.UseStaticFiles();
        app.UseWebSockets();

        var group = app.MapGroup(pattern);
        ui.MapPages(group);

        return app;
    }

    public static void MapPage<T>(
        this RouteGroupBuilder group,
        UI<T> ui,
        [StringSyntax("Route")] string pattern,
        Action<UI<T>.Context> action) where T : IViewModel
    {
        group.MapGet(pattern, async httpContext =>
        {
            var xeroContext = UI<T>.XeroMemoryCache.Get(httpContext);

            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                // TODO: This is almost correct.  Work across multiple browsers but multiple tabs gets its Action stolen.
                // Rework this once you figure out the various ViewModel state levels.
                xeroContext.ViewModel.OnChanged = async () => await ui.Recompose(xeroContext);

                using (new HotReloadContext<T>(ui, xeroContext))
                {
                    await xeroContext.AssignWebSocket(httpContext.WebSockets);
                }
            }
            else if (xeroContext.webSocket == null || xeroContext.webSocket.State != WebSocketState.Open)
            {
                using (xeroContext.ViewModel.Batch())
                {
                    action(xeroContext);
                }

                // TODO: Optimize.  No need to convert to a single string when we 
                // have streams and pipes.
                await httpContext.Response.WriteAsync(ui.Compose(xeroContext).ToStringWithExtras());
            }
            else
            {
                // Looks like the browser already has the page AND a websocket.
                // Respond with a 204 - No Content which will not alter the page
                // and push down the new route.  After that run the lambda which
                // may or may not cause trigger mutations to be pushed to the browser.
                httpContext.Response.StatusCode = 204; // No Content
                // httpContext.Response.StatusCode = 214; // Transformation Applied (deprecated)
                await httpContext.Response.CompleteAsync();
                await xeroContext.Push($"window.history.pushState({{}},'', '{httpContext.Request.Path}')");
                using (xeroContext.ViewModel.Batch())
                {
                    action(xeroContext);
                }
            }
        });
    }
}
