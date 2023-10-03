partial class UI
{
    // Problem here, Func<T> isn't a value type.
    // Find another way to pass children without them jumping the gun onto the buffer.

    View Parent(params Func<View>[] children) => $"""
        <ol>
            <li>{children[0]()}</li>
            <li>{children[1]()}</li>
            <li>{children[2]()}</li>
        </ol>
        """;
}