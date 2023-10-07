partial class UI
{
    View Header() => $"""
        <section id="header" class="hstack">
            <div class="vstack">
                <img src="xero_logo.svg" />
            </div>
            <div class="vstack">
                <h1>
                    Build realtime web apps with <strong>zero</strong> JavaScript
                </h1>
                <h2>
                    Your browser as a dumb shell with a brilliant rendering engine
                </h2>
                <ul>
                    <li>
                        HTML renders server-side via raw string literal interpolation
                    </li>
                    <li>
                        Server listens to DOM events through a WebSocket
                    </li>
                    <li>
                        Server reacts to render-deltas by pushing DOM mutations
                    </li>
                </ul>
            </div>
        </section>
""";
}
