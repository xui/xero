partial class UI
{
    View Child(MyViewModel vm, string? name = null) => $"""
        <p>{name ?? vm.Name ?? "(none)"}</p>
        """;
}