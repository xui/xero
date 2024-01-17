partial class UI
{
    // Problem here, Func<T> isn't a value type.
    // Find another way to pass children without them jumping the gun onto the buffer.

    HtmlString HasContent(Func<HtmlString> content) => $"""
        <h2 style="color: red">
            {content()}
        </h2>
    """;

    HtmlString Slots(
        Func<HtmlString> title,
        Func<HtmlString> caption,
        Func<HtmlString> content
    ) => $"""
        <h2>{title()}</h2>
        <code>{caption()}</code>
        <p style="color: red">
            {content()}
        </p>
    """;

}