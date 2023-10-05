partial class UI
{
    View JavaScript(ViewModel vm) => $"""
        <section>
            <div class="card-hero">
                <button onclick="{Increment}">Clicks: {vm.Count}</button>
            </div>
            <div class="card-content">
                <h2 class="tag">
                    Simplify
                </h2>
                <h1>
                    <em>Zero</em> JavaScript
                </h1>
                <p>
                    I love JavaScript — been using it with ❤️‍🩹 since 1999. But JavaScript has a
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
            </div>
        </section>
        """;

    void Increment(Context context)
    {
        context.ViewModel.Count++;
    }
}
