partial class UI
{
    View Footer() => $"""
        <ol>
            <li>footer 1</li>
            <li>footer 2</li>
            <li>footer 3</li>
            <button onClick="{UpdateTheRecordsAsync}">I'm a footer</button>
        </ol>
        """;
}