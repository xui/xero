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

    // TODO: Connect() should be injected automatically 
    // once you have string literal parsing in place.  Then make this private.
    // TODO: Minimize this by sending the extra bits down the websocket after it's opened.
    protected string Connect()
    {
        return $$"""
            <script>
                function h(id,ev) {
                    console.debug("executing slot " + id);
                    if (ev) {
                        ws.send(`${id}${encodeEvent(ev)}`);
                    } else {
                        ws.send(id);
                    }
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

                function encodeEvent(e) {
                    const obj = {};
                    for (let k in e) { obj[k] = e[k]; }
                    return JSON.stringify(obj, (k, v) => {
                        // TODO: There are a few more properties that can be shaved off.
                        if (v instanceof Node) return {id: v.id,name: v.name,type: v.type,value: v.value};
                        if (v instanceof Window) return null;
                        return v;
                    }, '');
                }

                function replaceNode(node, content) {
                    const regScript = node.nextSibling;
                    node.outerHTML = content;
                    node = regScript.previousSibling;
                    reRegister(regScript);
                    for (let s of node.getElementsByTagName("script")) {
                        reRegister(s);
                    }
                }

                function reRegister(node) {
                    if (node.tagName == "SCRIPT") {
                        const s = document.createElement("script");
                        s.textContent = node.textContent;
                        node.replaceWith(s);
                    }
                }

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
                        const data = JSON.parse(event.data);
                        if (data?.type == "UpdateStaticFile") {
                            console.log("refreshing...");
                            ws.close();
                            location.reload();
                        }
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
                const s = document.currentScript;
                let n = s.previousSibling;
                if (n.nodeType==8) {
                    n = document.createTextNode("");
                    s.parentNode.insertBefore(n, s);
                }
                this[id] = n;
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