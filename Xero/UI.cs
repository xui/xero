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

    protected abstract View MainLayout(T viewModel);

    public View Compose(Context context)
    {
        return View.Create(context.ViewBuffer, $"{MainLayout(context.ViewModel)}");
    }

    public void Recompose(Context context)
    {
        var compare = View.Create(context.CompareBuffer, $"{MainLayout(context.ViewModel)}");
        context.PushMutations(ref compare);
    }

    // TODO: Connect() should be injected automatically 
    // once you have string literal parsing in place.  Then make this private.
    // TODO: Minimize this by sending the extra bits down the websocket after it's opened.
    protected string Connect()
    {
        return $$"""
            <script>
                var verbose = false;
                var l = location;
                const ws = new WebSocket(`ws://${l.host}${l.pathname}`);

                ws.onopen = (event) => {
                    if (verbose)
                        console.log("onopen: ", event);
                };

                ws.onmessage = (event) => {
                    if (verbose)
                        console.log("onmessage: ", event);
                    eval(event.data);
                };

                ws.onclose = (event) => {
                    if (verbose)
                        console.log("onclose", event);
                };

                ws.onerror = (event) => {
                    if (verbose)
                        console.error("onerror: ", event);
                };

                function e(id) {
                    if (verbose)
                        console.log("executing slot " + id);
                    ws.send(id);
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