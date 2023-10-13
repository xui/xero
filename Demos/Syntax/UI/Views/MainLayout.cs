using Xero;

partial class UI
{
    protected override HtmlString MainLayout(MyViewModel vm) => $"""
        <html>
            <head>
                <!-- Zero script refs.  Such fast. -->
                {Register()}
            </head>
            <body>
                <h2>Hot Reloads: {HotReload.ReloadCount}</h2>

                <h3>Child nodes: simple</h3>
                <p>{vm.Name ?? "(none)"}</p>

                <h3>Child nodes: mixed</h3>
                <p>
                    Hello {vm.Name ?? "(none)"}!
                </p>

                <h3>Supported primitives and formatting</h3>
                <ul>
                    <li>string: {vm.Name ?? "(none)"}</li>
                    <li>int: {vm.Count}</li>
                    <li>DateTime:H:mm:ss: {DateTime.Now}</li>
                    <li>DateTime:H:mm:ss: {DateTime.Now:H:mm:ss}</li>
                    <li>DateTime:0: {DateTime.Now:O}</li>
                </ul>

                <h3>Null-coalescing operator</h3>
                <p>
                    {vm.Name ?? "(not specified)"}
                </p>

                <h3>Conditional (ternary) operator</h3>
                <p>{vm.Count} {(vm.Count % 2 == 0 ? "is even" : "is odd")}</p>
                <p>
                    {(vm.Name == null
                        ? $"Welcome {vm.Name}"
                        : $"Please log in <button>Login</button>"
                    )}
                </p>

                <h3 onmouseover="{this.UpdateTheRecordThings}">Event listener expressions</h3>
                <button onClick="{this.UpdateTheRecordThings}">Click: {vm.Count}</button>
                <button onClick="{ctx => ctx.ViewModel.Count -= 10}">Click: {vm.Count}</button>
                <button onClick="{this.UpdateTheRecordsAsync}">Async</button>
                <button onClick="{this.StartTimer}">Tick</button>

                <h3>Links & 204s</h3>
                <ul>
                    <li><a href="some-page">https://xero.wtf/some-page</a></li>
                    <li><a href="other-page">https://xero.wtf/other-page</a></li>
                    <li><a href="last-page">https://xero.wtf/last-page</a></li>
                </ul>

                <h3>switch with</h3>
                <p>
                    This is new to me...
                </p>

                <h3>Async/await expressions</h3>
                <p>
                    
                </p>

                <h3>Component with children</h3>
                {Parent(
                    () => Child(vm, "one"),
                    () => Child(vm),
                    () => Child(vm, "three")
                )}

                <h3><span style="text-decoration: line-through">Conditionals with if statements</span> Inline blocks</h3>
                <p>
                    {_ =>
                    {
                        HtmlString message;
                        if (vm.Name != null)
                            message = $"Welcome {vm.Name}";
                        else
                            message = $"Please log in <button>Login</button>";
                        return $"<p class='message'>{message}</p>";
                    }}
                </p>

                <h3>Async/await blocks</h3>
                <p>
                    
                </p>

                <h3>Templates: simple</h3>
                {Header(vm)}

                <h3>Templates: parameters</h3>
                {Header(vm, nameOverride: "Custom name")}

                <h3>Rendering arrays</h3>
                <p></p>

                <h3>Repeating templates with map</h3>
                <p></p>

                <h3>Repeating templates with looping statements</h3>
                <p></p>

                <h3>The repeat directive (ForEach)</h3>
                <p></p>

                <h3>Built-in directives</h3>
                <p></p>

                <h3>Custom directives</h3>
                <p></p>

                <h3></h3>
                <p></p>






                <br/><br/><br/><br/><br/><br/>








                {Connect()}
            </body>
        </html>
        """;

    /*
                <h3>Caching template results: the cache directive</h3>
                <p>
                    (not supported)
                </p>

                <h3>Conditionally rending nothing</h3>
                <p>
                    (not yet supported)
                </p>

                <h3>Sentinel values</h3>
                <p>(not supported)</p>

                <h3>DOM nodes</h3>
                <p>(not supported)</p>

                <h3>Sentinel values</h3>
                <p>(not supported)</p>

                <h3>Attribute expressions: full value</h3>
                <p class="{vm.Color}">full value</p>

                <h3>Attribute expressions: partial value</h3>
                <p style="color: {vm.Color}">partial value</p>

                <h3>Boolean attributes</h3>
                <div ?hidden=${!vm.ShowAdditional}>This text may be hidden.</div>

                <h3>Removing an attribute</h3>
                <pre>{"""<img src="/images/${this.imagePath ?? nothing}/${this.imageFile ?? nothing}>"""}</pre>

                <h3>Property expressions</h3>
                <p>(not supported)</p>

                <h3>Event listener expressions</h3>
                <button onClick="{logic.UpdateTheRecordThings}">Click Me</button>

                <h3>Element expressions</h3>
                <pre><div $myDirective()></div></pre>

                <h3>Well-formed HTML</h3>
                <p>Ack, forgot the close div!  Can this be a compiler error?</p>

                <h3>Valid expression locations</h3>
                <p>Expressions can only occur where you can place attribute values and child elements in HTML.</p>

                <h3>Static expressions</h3>
                <p>i.e. literal``</p>

                <h3>Non-literal statics</h3>
                <p>???</p>












                <h1>WAT? {vm.Name}</h1>
                <p>Count: {vm.Count} {(vm.Count % 2 == 0 ? "time" : "times")}</p>
                <p>Hot Reloads: {HotReload.ReloadCount}</p>

                <h2>{DateTime.Now:H:mm:ss}</h2>
                <h3>{DateTime.Now:O}</h3>

                {Header()}

                {(vm.Count % 2 == 0 ? Header() : Footer())}
                
                {inline =>
                {
                    if (vm.Count % 2 == 0)
                    {
                        return Footer();
                    }
                    else
                    {
                        return $"<p>empty here</p>";
                    }
                }}

                <ul>
                    <li>{vm.Sub?.Email ?? "none"}</li>
                    <li>{vm.Sub?.First}</li>
                    <li>{vm.Sub?.Last}</li>
                    <li>{vm.Sub?.Password}</li>
                    <li>{vm.Sub?.Quantity}</li>
                </ul>

                <div>
                    <button onClick="{UpdateTheRecordsAsync}">Click to async BL as var</button>
                </div>
                {Footer()}
    */
}