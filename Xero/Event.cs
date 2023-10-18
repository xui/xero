public class Event
{
    public bool? altKey { get; set; }
    public int? button { get; set; }
    public int? buttons { get; set; }
    public TouchPoint[]? changedTouches { get; set; }
    public double? clientX { get; set; }
    public double? clientY { get; set; }
    public string? code { get; set; }
    public bool? ctrlKey { get; set; }
    public HtmlElement? currentTarget { get; set; }
    public string? data { get; set; }
    public DataTransfer? dataTransfer { get; set; }
    public long? deltaMode { get; set; }
    public double? deltaX { get; set; }
    public double? deltaY { get; set; }
    public double? deltaZ { get; set; }
    public long? detail { get; set; }
    public string? inputType { get; set; }
    public bool? isComposing { get; set; }
    public bool? isTrusted { get; set; }
    public string? key { get; set; }
    public double? layerX { get; set; }
    public double? layerY { get; set; }
    public string? locale { get; set; }
    public int? location { get; set; }
    public bool? metaKey { get; set; }
    public double? movementX { get; set; }
    public double? movementY { get; set; }
    public double? offsetX { get; set; }
    public double? offsetY { get; set; }
    public double? pageX { get; set; }
    public double? pageY { get; set; }
    public HtmlElement? relatedTarget { get; set; }
    public bool? repeat { get; set; }
    public double? screenX { get; set; }
    public double? screenY { get; set; }
    public bool? shiftKey { get; set; }
    public HtmlElement? target { get; set; }
    public TouchPoint[]? targetTouches { get; set; }
    public TouchPoint[]? touches { get; set; }
    public string? type { get; set; }
    public double? x { get; set; }
    public double? y { get; set; }
}

public class HtmlElement
{
    public string? id { get; set; }
    public string? name { get; set; }
    public string? type { get; set; }
    public string? value { get; set; }
}

public class DataTransfer
{
    public string? dropEffect { get; set; }
    public string? effectAllowed { get; set; }
    public string[]? files { get; set; }
    public DataTransferItem[]? items { get; set; }
    public string[]? types { get; set; }
}

public class DataTransferItem
{
    public string? kind { get; set; }
    public string? type { get; set; }
}

public class TouchPoint
{
    public long? identifier { get; set; }
    public double? screenX { get; set; }
    public double? screenY { get; set; }
    public double? clientX { get; set; }
    public double? clientY { get; set; }
    public double? pageX { get; set; }
    public double? pageY { get; set; }
}
