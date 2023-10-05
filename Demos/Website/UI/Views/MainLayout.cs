partial class UI
{
    protected override View MainLayout(ViewModel vm) => $"""
        <html>
        <head>
            <!-- Zero script refs.  Such fast. -->
            <link rel="stylesheet" href="/css/site.css" />
            {Register()}
        </head>
        <body>
            {Header()}
            {VSCode()}
            {JavaScript(vm)}
            {Syntax(vm)}
            {Hooks(vm)}
            {Api(vm)}
            {Blocking(vm)}
            {Seo(vm)}
            {Thrashing(vm)}
            {VirtualDom(vm)}
            {Latency(vm)}
            {Allocations(vm)}
            {HotReloads(vm)}
            {Repl(vm)}
            {Pages(vm)}
            {Benchmarks(vm)}
            {Stupid(vm)}
            {Roadmap(vm)}

            {Connect()}
            {Watch()}
        </body>
        </html>
        """;
}
