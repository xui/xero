using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Diagnostics.CodeAnalysis;

namespace Xero;

public abstract partial class UI<T> where T : IViewModel
{
    protected UI()
    {
    }

    protected abstract HtmlString MainLayout(T viewModel);

    public HtmlString Compose(Context context)
    {
        return HtmlString.Create(context.ViewBuffer, $"{MainLayout(context.ViewModel)}");
    }

    public void Recompose(Context context)
    {
        var compare = HtmlString.Create(context.CompareBuffer, $"{MainLayout(context.ViewModel)}");
        context.PushMutations(ref compare);
    }

    // TODO: Connect() should be injected automatically 
    // once you have string literal parsing in place.  Then make this private.
    // TODO: Minimize this by sending the extra bits down the websocket after it's opened.
    protected string Connect()
    {
        return $$"""
            <script>
                function e(id) {
                    console.debug("executing slot " + id);
                    ws.send(id);
                }

                function debugSocket(name, ws) {
                    ws.onopen = (event) => { console.debug(`${name} onopen`, event); };
                    ws.onclose = (event) => { console.debug(`${name} onclose`, event); };
                    ws.onerror = (event) => { console.error(`${name} onerror`, event); };
                }

                var l = location;
                const ws = new WebSocket(`ws://${l.host}${l.pathname}`);
                debugSocket("xero", ws);
                ws.onmessage = (event) => {
                    console.debug("onmessage: ", event);
                    eval(event.data);
                };
            </script>
    """;
    }

    protected string Watch()
    {
        var endpoints = Environment.GetEnvironmentVariable("ASPNETCORE_AUTO_RELOAD_WS_ENDPOINT")!;

        if (string.IsNullOrWhiteSpace(endpoints))
            return string.Empty;

        return $$"""
            <script>
                let dnw; // static file change
                for (const url of '{{endpoints}}'.split(',')) {
                    try {
                        dnw = new WebSocket(url);
                        break;
                    } catch (ex) {
                        console.debug(ex);
                    }
                }
                if (dnw) {
                    debugSocket("dotnet-watch", dnw);
                    dnw.onmessage = (event) => {
                        console.debug("onmessage: ", event);
                        ws.close();
                        location.reload();
                    };
                } else {
                    console.debug('Unable to establish a connection to the dotnet watch browser refresh server.');
                }
            </script>
    """;
    }

    // TODO: Register() should be injected automatically
    // as an only-once js-registration functionality.  Then make this private.
    protected static string Register() => """
        <script>
            function r(id) {
                this[id] = document.currentScript.previousSibling;
            }
        </script>
        """;

    public virtual void MapPages()
    {
        // This is optionally implemented in dev's concrete UI class.
        // Dev will call MapGet() from here.

        MapPage("/", ctx => { });
    }

    // This mess will eventually be replaced with a source generator
    // so devs can just decorate their methods with routes.  Pretty!
    private RouteGroupBuilder? group;
    internal void MapPages(RouteGroupBuilder group)
    {
        this.group = group;
        MapPages();
        this.group = null;
    }

    protected void MapPage([StringSyntax("Route")] string pattern, Action<Context> action)
    {
        group?.MapPage(this, pattern, action);
    }
}