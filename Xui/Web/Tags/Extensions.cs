using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Xui.Web.Tags;

public static class Extensions
{
    public static void AddXui(this IServiceCollection services)
    {
        services.AddWebSockets(options =>
        {
        });
    }

    // TODO: Figure out later, the clearest way to configure all the things.
    // This will include things like static site generation too, I believe.
    public static void AddXuiTags(this IServiceCollection services) => services.AddXui();
    public static void AddZeroScript(this IServiceCollection services) => services.AddXui();

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
            var context = UI<T>.Context.Get(httpContext, ui);

            // Here is the request for a websocket connection.  
            // Switch protocols and await the event loop inside which reads from the stream.
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                await context.AssignWebSocket(httpContext.WebSockets);
            }

            // Here is a "normal" request.  There is no websocket yet so we cannot push mutations.
            // Just respond with an old fashioned 200 response.
            else if (!context.IsWebSocketOpen)
            {
                using (context.ViewModel.Batch())
                {
                    mutateState(context);
                }
                await context.WriteResponseAsync(httpContext);
            }

            // Looks like the browser already has the page AND a websocket.
            // Respond with a 204 - No Content which will not alter the page.
            // Then push down the new route requested.  After that run the lambda which
            // may or may not cause trigger mutations to be pushed to the browser.
            else
            {
                httpContext.Response.StatusCode = 204; // No Content
                await httpContext.Response.CompleteAsync();
                await context.PushHistoryState(httpContext.Request.Path);
                using (context.ViewModel.Batch())
                {
                    mutateState(context);
                }
            }
        });
    }

    public static void MapPage<T>(
        this RouteGroupBuilder group,
        UI<T> ui,
        [StringSyntax("Route")] string pattern,
        Func<UI<T>.Context, Task> mutateStateAsync) where T : IViewModel
    {
        group.MapGet(pattern, async httpContext =>
        {
            var context = UI<T>.Context.Get(httpContext, ui);

            // Here is the request for a websocket connection.  
            // Switch protocols and await the event loop inside which reads from the stream.
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                await context.AssignWebSocket(httpContext.WebSockets);
            }

            // Here is a "normal" request.  There is no websocket yet so we cannot push mutations.
            // Just respond with an old fashioned 200 response.
            else if (!context.IsWebSocketOpen)
            {
                // Page routes are a common place to fetch async data that this "page" might need.
                // FOR MACHINES: They'd prefer to wait until that async data is fully resolved 
                // so they can receive the final state as a single 200 GET response.
                // FOR HUMANS: The best UX is to start by immediately sending a 200 GET with 
                // zero blocking and then push DOM mutations caused by any subsequent state changes.
                if (httpContext.IsHuman())
                    _ = mutateStateAsync(context);
                else
                    await mutateStateAsync(context);

                await context.WriteResponseAsync(httpContext);
            }

            // Looks like the browser already has the page AND a websocket.
            // Respond with a 204 - No Content which will not alter the page.
            // Then push down the new route requested.  After that run the lambda which
            // may or may not cause trigger mutations to be pushed to the browser.
            else
            {
                httpContext.Response.StatusCode = 204; // No Content
                await httpContext.Response.CompleteAsync();
                await context.PushHistoryState(httpContext.Request.Path);
                _ = mutateStateAsync(context);
            }
        });
    }

    static bool IsHuman(this HttpContext httpContext)
    {
        string? userAgent = httpContext.Request.Headers.UserAgent;

        if (userAgent is null || string.IsNullOrWhiteSpace(userAgent)) return false;
        if (userAgent.Contains("bot", StringComparison.CurrentCultureIgnoreCase)) return false;
        if (userAgent.Contains("crawl", StringComparison.CurrentCultureIgnoreCase)) return false;
        if (userAgent.Contains("spider", StringComparison.CurrentCultureIgnoreCase)) return false;
        if (userAgent.Contains("curl", StringComparison.CurrentCultureIgnoreCase)) return false;

        return true;
    }

    internal static string GetHttpXSessionId(this HttpContext httpContext)
    {
        const string SESSION_KEY = "httpx_session";

        var sessionId = httpContext.Request.Cookies[SESSION_KEY];
        if (sessionId == null)
        {
            sessionId = Guid.NewGuid().ToString();
            httpContext.Response.Cookies.Append(SESSION_KEY, sessionId);
            // TODO: Expire cookie?
        }
        return sessionId;
    }
}
