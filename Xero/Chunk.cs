using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Xero;

internal struct Chunk
{
    public int Id;
    public FormatType Type;
    public string? String;
    public int? Integer;
    public bool? Boolean;
    public DateTime? DateTime;
    public IView? View;
    public string? Format;
    public Action Action;
    public Action<Event> ActionEvent;
    public Func<Task> ActionAsync;
    public Func<Event, Task> ActionEventAsync;

    public static bool operator ==(Chunk c1, Chunk c2)
    {
        if (c1.Type != c2.Type)
            return false;

        switch (c1.Type)
        {
            case FormatType.StringLiteral:
                return c1.String == c2.String;
            case FormatType.String:
                return c1.String == c2.String && c1.Format == c2.Format;
            case FormatType.Integer:
                return c1.Integer == c2.Integer && c1.Format == c2.Format;
            case FormatType.DateTime:
                return c1.DateTime == c2.DateTime && c1.Format == c2.Format;
            case FormatType.Boolean:
                return c1.Boolean == c2.Boolean && c1.Format == c2.Format;
            case FormatType.View:
                return c1.View == c2.View;
            case FormatType.HtmlString:
                // no-op
                return true;
        }

        return true;
    }

    public static bool operator !=(Chunk c1, Chunk c2)
    {
        return !(c1 == c2);
    }

    public override readonly bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    internal readonly void Append(StringBuilder builder)
    {
        switch (this.Type)
        {
            case FormatType.StringLiteral:
                builder.Append(this.String);
                break;
            case FormatType.String:
                if (this.Format is null)
                    builder.Append(this.String);
                else
                    builder.AppendFormat($"{{0:{this.Format}}}", this.String);
                break;
            case FormatType.Integer:
                if (this.Format is null)
                    builder.Append(this.Integer);
                else
                    builder.AppendFormat($"{{0:{this.Format}}}", this.Integer);
                break;
            case FormatType.Boolean:
                if (this.Format is null)
                    builder.Append(this.Boolean);
                else
                    builder.AppendFormat($"{{0:{this.Format}}}", this.Boolean);
                break;
            case FormatType.DateTime:
                if (this.Format is null)
                    builder.Append(this.DateTime);
                else
                    builder.AppendFormat($"{{0:{this.Format}}}", this.DateTime);
                break;
            case FormatType.View:
            case FormatType.HtmlString:
                // no-op
                break;
            case FormatType.Action:
            case FormatType.ActionAsync:
                builder.Append("e(");
                builder.Append(this.Id);
                builder.Append(")");
                break;
            case FormatType.ActionEvent:
            case FormatType.ActionEventAsync:
                builder.Append("e(");
                builder.Append(this.Id);
                builder.Append(",event)");
                break;
            default:
                throw new Exception($"Unsupported type: {this.Type}");
        }
    }
}