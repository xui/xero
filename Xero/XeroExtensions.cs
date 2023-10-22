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
        Action<UI<T>.Context> mutateState) where T : IViewModel
    {
        group.MapGet(pattern, async httpContext =>
        {
            var xeroContext = UI<T>.XeroMemoryCache.Get(httpContext);

            // Here is the request for a websocket connection.  
            // Switch protocols and await the event loop inside which reads from the stream.
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                await xeroContext.AssignWebSocket(httpContext.WebSockets, ui);
            }

            // Here is a "normal" request.  There is no websocket yet so we cannot push mutations.
            // Just respond with an old fashioned 200 response.
            else if (xeroContext.webSocket == null || xeroContext.webSocket.State != WebSocketState.Open)
            {
                using (xeroContext.ViewModel.Batch())
                {
                    mutateState(xeroContext);
                }

                await xeroContext.WriteResponseAsync(httpContext, ui);
            }

            // Looks like the browser already has the page AND a websocket.
            // Respond with a 204 - No Content which will not alter the page.
            // Then push down the new route requested.  After that run the lambda which
            // may or may not cause trigger mutations to be pushed to the browser.
            else
            {
                httpContext.Response.StatusCode = 204; // No Content
                await httpContext.Response.CompleteAsync();

                await xeroContext.Push($"window.history.pushState({{}},'', '{httpContext.Request.Path}')");

                using (xeroContext.ViewModel.Batch())
                {
                    mutateState(xeroContext);
                }
            }
        });
    }
}
