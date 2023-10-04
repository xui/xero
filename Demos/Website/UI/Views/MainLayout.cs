using Xero;

partial class UI
{
    protected override View MainLayout(MyViewModel vm) => $"""
        <html>
            <head>
                <!-- Zero script refs.  Such fast. -->
                {Register()}
            </head>
            <body>
                <h1>Build realtime web apps with <em>zero</em> JavaScript</h1>
                <h2>What would web development look like if it were reconsidered from first principles by mobile developers?</h2>
                {Connect()}
            </body>
        </html>
        """;
}