using System.Runtime.CompilerServices;
using System.Text;

[InterpolatedStringHandler]
public struct Placebo
{
    public Placebo(int literalLength, int formattedCount)
    {
    }

    public readonly void AppendLiteral(string s)
    {
    }

    public readonly void AppendFormatted(string? s, string? format = null)
    {
    }

    public readonly void AppendFormatted(int? i, string? format = null)
    {
    }

    public readonly void AppendFormatted(bool? b, string? format = null)
    {
    }

    public readonly void AppendFormatted(DateTime? d, string? format = null)
    {
    }

    public readonly void AppendFormatted(Placebo s)
    {
    }
}