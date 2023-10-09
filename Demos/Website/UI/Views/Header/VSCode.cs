partial class UI
{
    View VSCode() => $"""
        <section class="vscode">
            <div class="sidebar">
                <details open>
                    <summary>Views</summary>
                    <details>
                        <summary>Cards</summary>
                        {File("Allocations.cs")}
                        {File("Api.cs")}
                        {File("Benchmarks.cs")}
                        {File("Blocking.cs")}
                        {File("Hooks.cs")}
                        {File("HotReloads.cs")}
                        {File("JavaScript.cs")}
                        {File("Latency.cs")}
                        {File("Pages.cs")}
                        {File("Repl.cs")}
                        {File("Roadmap.cs")}
                        {File("Seo.cs")}
                        {File("Stupid.cs")}
                        {File("Syntax.cs")}
                        {File("Thrashing.cs")}
                        {File("VirtualDom.cs")}
                    </details>
                    <details>
                        <summary>Header</summary>
                        {File("Header.cs")}
                        {File("VSCode.cs")}
                    </details>
                    {File("MainLayout.cs")}
                </details>
                {File("UI.cs")}
                {File("ViewModel.cs")}
            </div>
            <div class="code">
                <img src="/img/vscode_logo.svg" />
                <p>
                    VS Code &#x2764; C#
                </p>
            </div>
        </section>
        """;

    View File(string name) => $"""
        <label for="{name}">
            <input type="radio" name="file" id="{name}">{name}</input>
        </label>
        """;
}
