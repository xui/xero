partial class UI
{
    View VSCode() => $"""
        <section class="vscode">
            <div class="sidebar">
                <details open>
                    <summary>Views</summary>
                    {File("Child.cs")}
                    {File("Footer.cs")}
                    {File("Header.cs")}
                    {File("Header.cs")}
                    {File("Header.cs")}
                    {File("Header.cs")}
                    {File("Header.cs")}
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
