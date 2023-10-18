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
    public string? id;
    public string? name;
    public string? type;
    public string? value;
}

public class DataTransfer
{
    public string? dropEffect;
    public string? effectAllowed;
    public string[]? files;
    public DataTransferItem[]? items;
    public string[]? types;
}

public class DataTransferItem
{
    public string? kind;
    public string? type;
}

public class TouchPoint
{
    public long? identifier;
    public double? screenX;
    public double? screenY;
    public double? clientX;
    public double? clientY;
    public double? pageX;
    public double? pageY;
}
