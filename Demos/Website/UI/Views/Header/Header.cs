partial class UI
{
    View Header() => $"""
        <section id="header" class="hstack">
            <div class="vstack">
                <img src="img/xero_logo.svg" />
            </div>
            <div class="vstack">
                <h1>
                    Build realtime web apps with <strong>zero</strong> JavaScript
                </h1>
                <h2>
                    Your browser makes an excellent thin client
                </h2>
                <ul>
                    <li>
                        Server renders HTML using raw string literal interpolations
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
