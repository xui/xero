partial class UI
{
    HtmlString Child(MyViewModel vm, string? name = null) => $"""
        <p>{name ?? vm.Name ?? "(none)"}</p>
        """;
}