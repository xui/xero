readonly record struct Api() : IView
{
    public HtmlString Render() => $"""
        <section id="zero-api">
            <div class="vstack">
                <img src="img/3d_rotate.png" />
                <p>Scan to rotate this QR code with your phone's gyroscope!</p>
            </div>
            <article>
                <h2>
                    Simplify
                </h2>
                <h1>
                    <strong>Zero</strong> APIs to build
                </h1>
                <p>
                    Usually building a SPA comes with double the work since running your 
                    logic in the browser requires you to also build an...
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
