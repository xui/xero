public class Event
{
    public HtmlElement? currentTarget;
    public bool? isTrusted;
    public HtmlElement? target;
    public string? type;
}

public class UIEvent : Event
{
    public long? detail;
}

public class MouseEvent : UIEvent
{
    public bool? altKey;
    public int? button;
    public int? buttons;
    public double? clientX;
    public double? clientY;
    public bool? ctrlKey;
    public double? layerX;
    public double? layerY;
    public bool? metaKey;
    public double? movementX;
    public double? movementY;
    public double? offsetX;
    public double? offsetY;
    public double? pageX;
    public double? pageY;
    public HtmlElement? relatedTarget;
    public double? screenX;
    public double? screenY;
    public bool? shiftKey;
    public double? x;
    public double? y;
}

public class DragEvent : MouseEvent
{
    public DataTransfer? dataTransfer;
}

public class WheelEvent : MouseEvent
{
    public double? deltaX;
    public double? deltaY;
    public double? deltaZ;
    public long? deltaMode;
}

public class TouchEvent : UIEvent
{
    public bool? altKey;
    public TouchPoint[]? changedTouches;
    public bool? ctrlKey;
    public bool? metaKey;
    public bool? shiftKey;
    public TouchPoint[]? targetTouches;
    public TouchPoint[]? touches;
}

public class FocusEvent : UIEvent
{
    public HtmlElement? relatedTarget;
}

public class KeyboardEvent : UIEvent
{
    public bool? altKey;
    public string? code;
    public bool? ctrlKey;
    public bool? isComposing;
    public string? key;
    public string? locale;
    public int? location;
    public bool? metaKey;
    public bool? repeat;
    public bool? shiftKey;
}

public class InputEvent : UIEvent
{
    public string? data;
    public DataTransfer? dataTransfer;
    public string? inputType;
    public bool? isComposing;
}

public class CompositionEvent : UIEvent
{
    public string? data;
    public string? locale;
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
