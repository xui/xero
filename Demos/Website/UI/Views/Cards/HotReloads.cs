readonly record struct HotReloads() : IView
{
    public HtmlString Render() => $"""
        <section id="hot-reload">
            <div>
                <h1>
                    {Xero.HotReload.ReloadCount}
                </h1>
                <h2>
                    Hot Reloads
                </h2>
            </div>
            <article>
                <h2>
                    Developer Experience
                </h2>
                <h1>
                    Hot Reload
                </h1>
                <p>
                    Use `dotnet watch` while you're developing and see your changes 
                    instantly without losing any state.
                </p>
                <p>
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod
                    tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam,
                    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
                    consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse
                    cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat
                    non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                </p>
                <p>
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod
                    tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam,
                    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
                    consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse
                    cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat
                    non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                </p>
            </article>
        </section>
        """;
}
