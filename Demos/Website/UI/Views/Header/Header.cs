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
                    Xero treats your web browser like a thin client.
                </h2>
                <ul>
                    <li>
                        HTML is always rendered server-side
                    </li>
                    <li>
                        DOM events piped to the server via WebSocket
                    </li>
                    <li>
                        Xero reacts to render-deltas with DOM mutations
                    </li>
                </ul>
            </div>
        </section>
""";
}
