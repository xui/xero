partial class UI
{
    protected override HtmlString MainLayout(ViewModel vm) => $"""
        <html>
        <head>
            <!-- Zero script-refs.  Such fast. -->

            <link rel="stylesheet" href="/css/preflight.css" />
            <link rel="stylesheet" href="/css/site.css" />
            {Register()}
        </head>
        <body>
            <nav>
            </nav>

            {new Header()}
            {new VSCode()}

            <div id="cards">
                {new JavaScript(vm)}
                {new Syntax()}
                {new Api()}
                {new Hooks()}
                {new Blocking()}
                {new Seo()}
                {new VirtualDom()}
                {new Latency()}
                {new Allocations()}
                {new HotReloads()}
                {new Repl()}
                {new Pages()}
                {new Benchmarks()}
                {new Sense()}
                {new Roadmap()}
            </div>

            {Connect()}
            {Watch()}
        </body>
        </html>
        """;
}
