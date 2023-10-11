partial class UI
{
    View Hooks(ViewModel vm) => $$"""
        <section id="zero-hooks">
            <div class="vstack">
                <div class="hstack>
                    <img src="img/drake_no.jpg" />
                    <pre>
                        <div>const [name, setName] = useState('');</div>
                        <div>const [name, setName] = useState('');</div>
                    </pre>
                </div>
                <h1 />
                <div class="hstack>
                    <img src="img/drake_yes.jpg" />
                    <pre>
                        <div><p>{viewModel.Name ?? "(none)"}</p></div>
                        <div><p>{viewModel.Count * 2}</p></div>
                    </pre>
                </div>
            </div>
            <article>
                <h2>
                    Simplify
                </h2>
                <h1>
                    <strong>Zero</strong> $: or use-effects
                </h1>
                <p>
                    Managing state is the hardest part of any UI framework.  This is true for 
                    both web and native toolkits.  Derived values make this especially...
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
