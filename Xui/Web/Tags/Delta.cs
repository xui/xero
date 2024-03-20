namespace Xui.Web.Html;

public record struct Delta(
    int Id,
    DeltaType Type,
    string ValueAsString // TODO: Optimize from string to whatever the WebSocket wants.
);

public enum DeltaType
{
    NodeValue,
    NodeAttribute,
    HtmlPartial,
}