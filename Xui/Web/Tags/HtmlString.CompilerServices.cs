using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xui.Web.Html;

[InterpolatedStringHandler]
[StructLayout(LayoutKind.Auto)]
public partial struct HtmlString
{
    [ThreadStatic] static Composition? root;

    readonly int start;
    internal int end;

    readonly int goalLiteral;
    readonly int goalFormatted;
    int progressLiteral;
    int progressFormatted;

    internal static HtmlString Create(Composition composition, [InterpolatedStringHandlerArgument("composition")] HtmlString htmlString)
    {
        return htmlString;
    }

    public HtmlString(int literalLength, int formattedCount)
    {
        if (root is null)
            throw new ArgumentException("Root chunk not allowed without supplied composition.");
        // root ??= new();
        composition = root;

        composition.depth++;
        start = composition.cursor;
        end = start;

        goalLiteral = literalLength;
        goalFormatted = formattedCount;

        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.Integer = start;
        chunk.Type = FormatType.HtmlString;
        end++;
    }

    internal HtmlString(int literalLength, int formattedCount, Composition composition)
    {
        root = composition;
        this.composition = composition;

        composition.depth++;
        start = composition.cursor;
        end = start;

        goalLiteral = literalLength;
        goalFormatted = formattedCount;
    }

    private void MoveNext()
    {
        end++;
        composition.cursor = end;

        if (progressLiteral == goalLiteral && progressFormatted == goalFormatted)
        {
            Clear();
        }
    }

    private void Clear()
    {
        if (--composition.depth == 0)
        {
            composition.cursor = 0;
            root = null;
        }
    }

    public void AppendLiteral(string s)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.String = s;
        chunk.Integer = start;
        chunk.Type = FormatType.StringLiteral;

        progressLiteral += s.Length;
        MoveNext();
    }

    public void AppendFormatted(string s, string? format = null)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.String = s;
        chunk.Type = FormatType.String;
        chunk.Format = format;

        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted(int? i, string? format = null)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.Integer = i;
        chunk.Type = FormatType.Integer;
        chunk.Format = format;

        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted(bool? b, string? format = null)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.Boolean = b;
        chunk.Type = FormatType.Boolean;
        chunk.Format = format;

        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted(DateTime? d, string? format = null)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.DateTime = d;
        chunk.Type = FormatType.DateTime;
        chunk.Format = format;

        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted<TView>(TView v) where TView : IView
    {
        AppendFormatted(v.Render());
    }

    public void AppendFormatted(HtmlString h)
    {
        end = h.end;

        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.Integer = h.start;
        chunk.Type = FormatType.HtmlString;

        ref var start = ref composition.chunks[h.start];
        start.Integer = end;

        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted(Func<HtmlString> f)
    {
        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted(Func<string, HtmlString> f)
    {
        progressFormatted++;
        MoveNext();
    }

    // public void AppendFormatted<T>(T t)
    // {
    // }

    public void AppendFormatted(Action a)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.Action = a;
        chunk.Type = FormatType.Action;

        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted(Action<Event> a)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.ActionEvent = a;
        chunk.Type = FormatType.ActionEvent;

        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted(Func<Task> f)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.ActionAsync = f;
        chunk.Type = FormatType.ActionAsync;

        progressFormatted++;
        MoveNext();
    }

    public void AppendFormatted(Func<Event, Task> f)
    {
        ref var chunk = ref composition.chunks[end];
        chunk.Id = end;
        chunk.ActionEventAsync = f;
        chunk.Type = FormatType.ActionEventAsync;

        progressFormatted++;
        MoveNext();
    }
}