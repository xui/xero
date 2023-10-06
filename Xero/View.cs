using System.Runtime.CompilerServices;
using System.Text;

namespace Xero;

public abstract partial class UI<T>
{
    [InterpolatedStringHandler]
    public struct View
    {
        [ThreadStatic] static Buffer? rootBuffer;

        internal class Buffer
        {
            internal int cursor = 0;
            internal Chunk[] chunks = new Chunk[1000];
            internal int depth = 0;
        }

        readonly Buffer buffer;

        readonly int start;
        int end;

        readonly int goalLiteral;
        readonly int goalFormatted;
        int progressLiteral;
        int progressFormatted;

        internal static View Create(Buffer buffer, [InterpolatedStringHandlerArgument("buffer")] View view)
        {
            return view;
        }

        public View(int literalLength, int formattedCount)
        {
            if (rootBuffer is null)
                throw new ArgumentException("Root chunk not allowed without supplied buffer.");
            // rootBuffer ??= new();
            buffer = rootBuffer;

            buffer.depth++;
            start = buffer.cursor;
            end = start;

            goalLiteral = literalLength;
            goalFormatted = formattedCount;
        }

        internal View(int literalLength, int formattedCount, Buffer buffer)
        {
            rootBuffer = buffer;
            this.buffer = buffer;

            buffer.depth++;
            start = buffer.cursor;
            end = start;

            goalLiteral = literalLength;
            goalFormatted = formattedCount;
        }

        private void MoveNext()
        {
            end++;
            buffer.cursor = end;

            if (progressLiteral == goalLiteral && progressFormatted == goalFormatted)
            {
                Clear();
            }
        }

        private void Clear()
        {
            if (--buffer.depth == 0)
            {
                buffer.cursor = 0;
                rootBuffer = null;
            }
        }

        public void AppendLiteral(string s)
        {
            ref var chunk = ref buffer.chunks[end];
            chunk.Id = end;
            chunk.String = s;
            chunk.Type = FormatType.StringLiteral;

            progressLiteral += s.Length;
            MoveNext();
        }

        public void AppendFormatted(string s, string? format = null)
        {
            ref var chunk = ref buffer.chunks[end];
            chunk.Id = end;
            chunk.String = s;
            chunk.Type = FormatType.String;
            chunk.Format = format;

            progressFormatted++;
            MoveNext();
        }

        public void AppendFormatted(int? i, string? format = null)
        {
            ref var chunk = ref buffer.chunks[end];
            chunk.Id = end;
            chunk.Integer = i;
            chunk.Type = FormatType.Integer;
            chunk.Format = format;

            progressFormatted++;
            MoveNext();
        }

        public void AppendFormatted(bool? b, string? format = null)
        {
            ref var chunk = ref buffer.chunks[end];
            chunk.Id = end;
            chunk.Boolean = b;
            chunk.Type = FormatType.Boolean;
            chunk.Format = format;

            progressFormatted++;
            MoveNext();
        }

        public void AppendFormatted(DateTime? d, string? format = null)
        {
            ref var chunk = ref buffer.chunks[end];
            chunk.Id = end;
            chunk.DateTime = d;
            chunk.Type = FormatType.DateTime;
            chunk.Format = format;

            progressFormatted++;
            MoveNext();
        }

        public void AppendFormatted(View s)
        {
            end = s.end;

            ref var chunk = ref buffer.chunks[end];
            chunk.Id = end;
            chunk.Type = FormatType.HtmlString;

            progressFormatted++;
            MoveNext();
        }

        public void AppendFormatted(Func<View> f)
        {
            progressFormatted++;
            MoveNext();
        }

        public void AppendFormatted(Func<string, View> f)
        {
            progressFormatted++;
            MoveNext();
        }

        // public void AppendFormatted<T>(T t)
        // {
        // }

        public void AppendFormatted(Action<Context> a)
        {
            ref var chunk = ref buffer.chunks[end];
            chunk.Id = end;
            chunk.Action = a;
            chunk.Type = FormatType.Action;

            progressFormatted++;
            MoveNext();
        }

        public void AppendFormatted(Func<Context, Task> f)
        {
            ref var chunk = ref buffer.chunks[end];
            chunk.Id = end;
            chunk.ActionAsync = f;
            chunk.Type = FormatType.ActionAsync;

            progressFormatted++;
            MoveNext();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int i = start; i < end; i++)
            {
                var chunk = buffer.chunks[i];
                chunk.Append(builder);
            }
            return builder.ToString();
        }

        public string ToStringWithExtras()
        {
            bool probablyAnAttributeNextHack = false;

            var builder = new StringBuilder();
            for (int i = start; i < end; i++)
            {
                var chunk = buffer.chunks[i];

                switch (chunk.Type)
                {
                    case FormatType.Boolean:
                    case FormatType.DateTime:
                    case FormatType.Integer:
                    case FormatType.String:
                        if (probablyAnAttributeNextHack)
                        {
                            chunk.Append(builder);
                        }
                        else
                        {
                            builder.Append("<!-- -->");
                            chunk.Append(builder);
                            builder.Append("<script>r(\"slot");
                            builder.Append(chunk.Id);
                            builder.Append("\")</script>");
                        }
                        break;
                    default:
                        chunk.Append(builder);
                        break;
                }

                if (chunk.Type == FormatType.StringLiteral && chunk.String?[^1] == '"')
                {
                    probablyAnAttributeNextHack = true;
                }
                else
                {
                    probablyAnAttributeNextHack = false;
                }
            }
            return builder.ToString();
        }

        internal IEnumerable<Chunk> GetDeltas(View.Buffer buffer, View.Buffer compare)
        {
            for (int i = 0; i < end; i++)
            {
                if (buffer.chunks[i] != compare.chunks[i])
                {
                    yield return compare.chunks[i];
                }
            }
        }
    }
}

