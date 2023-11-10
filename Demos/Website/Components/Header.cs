readonly record struct Header() : IView
{
    public HtmlString Render() => $"""
        <section id="header" class="hstack">

            <img src="img/xero_logo.svg" />
            
            <div>
                <h1>
                    Build realtime web apps with <strong>zero</strong> JavaScript
                </h1>
                <h2>
                    Web browsers can be incredible thin clients
                </h2>
                <ul>
                    <li>
                        Server renders all HTML with fast raw string literal interpolations
                    </li>
                    <li>
                        Server listens to browser events through a WebSocket
                    </li>
                    <li>
                        Server reactively pushes mutations to the browser
                    </li>
                </ul>
            </div>
        </section>
""";
}
