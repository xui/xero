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

            {Header()}
            {VSCode()}

            <div id="cards">
                {JavaScript(vm)}
                {Syntax(vm)}
                {Hooks(vm)}
                {Api(vm)}
                {Blocking(vm)}
                {Seo(vm)}
                {VirtualDom(vm)}
                {Latency(vm)}
                {Allocations(vm)}
                {HotReloads(vm)}
                {Repl(vm)}
                {Pages(vm)}
                {Benchmarks(vm)}
                {Sense(vm)}
                {Roadmap(vm)}
            </div>

            {Connect()}
            {Watch()}
        </body>
        </html>
        """;
}
