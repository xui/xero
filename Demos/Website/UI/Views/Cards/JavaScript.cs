readonly record struct JavaScript(
    ViewModel ViewModel
) : IView
{
    public HtmlString Render() => $"""
        <section id="zero-javascript">
            <div>
                <button onclick="{Increment}">Clicks: {ViewModel.Count}</button>
            </div>
            <article>
                <h2>
                    Simplify
                </h2>
                <h1>
                    Write <strong>zero</strong> JavaScript
                </h1>
                <p>
                    I love JavaScript.  Been using it with &#x1F494; since 1999. But JavaScript has a
                    payload problem.
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

    void Increment(Event e)
    {
        ViewModel.Count++;

        Console.WriteLine($"x: {e.x}");
    }
}