using System.Text;

namespace Xui.Web.Html;

public partial struct HtmlString
{
    readonly Composition composition;

    public IEnumerable<Delta> Recompose(HtmlString compare)
    {
        List<Range>? ranges = null;
        for (int index = 0; index < end; index++)
        {
            var oldChunk = composition.chunks[index];
            var newChunk = compare.composition.chunks[index];
            if (oldChunk == newChunk) {
                continue;
            }

            ranges ??= [];

            if (newChunk.Type != FormatType.StringLiteral)
            {
                ranges.Add(new Range(newChunk.Id, newChunk.Id));
            }
            else
            {
                var htmlStringStart = compare.composition.chunks[newChunk.Integer!.Value];
                ranges.Add(new Range(newChunk.Integer.Value, htmlStringStart.Integer!.Value));
            }
        }

        if (ranges == null)
            yield break;

        ranges.Sort((a, b) => a.Start.Value - b.Start.Value);
        int i = 0, max = -1;
        while (i < ranges.Count)
        {
            var range = ranges[i];
            if (max >= range.Start.Value)
                ranges.RemoveAt(i);
            else
                i++;
            max = range.End.Value;
        }

        // TODO: Remove this StringBuilder once we know how we need to optimize for PushMutations & WebSockets.
        var ugh = new StringBuilder();
        foreach (var range in ranges)
        {
            var chunk = compare.composition.chunks[range.Start.Value];
            if (range.Start.Value == range.End.Value)
            {
                // Not a range, just a single value.  Mutate with nodeValue-precision.
                chunk.Append(ugh);
                yield return new Delta(
                    Id: chunk.Id,
                    Type: DeltaType.NodeValue, // TODO: Support attribute!
                    ValueAsString: ugh.ToString()
                );
            }
            else
            {
                // This is a range of changes.  Replace the whole HTML partial.
                // TODO: Optimize.  Any way to cleanly and efficiently trim and escape without calling ToString first?
                compare.ToStringWithExtras(range.Start.Value, range.End.Value - 1, ugh);
                yield return new Delta(
                    Id: chunk.Id,
                    Type: DeltaType.HtmlPartial,
                    ValueAsString: ugh.ToString().Trim()
                );
            }
            ugh.Clear();
        }
    }

    public void HandleEvent(int slotId, Event? domEvent)
    {
        // TODO: These should not block the Context.Receive event loop.
        // So none of these will be awaiting.  But that could cause some 
        // tricky overlapping.  I bet the user is expecting them to execute
        // in order?  Do I need a queue?  But this queue should belong to the Context?

        // TODO: Optimize.  Bypass the O(n).  Lazy Dict gets reset on each compose?
        var chunk = composition.chunks.First(c => c.Id == slotId);
        switch (chunk.Type)
        {
            case FormatType.Action:
                chunk.Action();
                break;
            case FormatType.ActionEvent:
                chunk.ActionEvent(domEvent ?? Event.Empty);
                break;
            case FormatType.ActionAsync:
                // Do not batch.  Mutations should go immediately.
                // Do not await. That'd block this event loop.
                _ = chunk.ActionAsync();
                break;
            case FormatType.ActionEventAsync:
                // Do not batch.  Mutations should go immediately.
                // Do not await. That'd block this event loop.
                _ = chunk.ActionEventAsync(domEvent ?? Event.Empty);
                break;
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        // -2 since we can ignore that final HtmlString chunk.
        for (int i = start; i < end - 2; i++)
        {
            var chunk = composition.chunks[i];
            chunk.Append(builder);
        }
        return builder.ToString();
    }

    public string ToStringWithExtras()
    {
        // -2 since we can ignore that final HtmlString chunk.
        var builder = new StringBuilder();
        ToStringWithExtras(start, end - 2, builder);
        return builder.ToString();
    }

    internal void ToStringWithExtras(int start, int end, StringBuilder builder)
    {
        bool hackProbablyAnAttributeNext = false;

        for (int i = start; i <= end; i++)
        {
            var chunk = composition.chunks[i];

            switch (chunk.Type)
            {
                case FormatType.Boolean:
                case FormatType.DateTime:
                case FormatType.Integer:
                case FormatType.String:
                    if (hackProbablyAnAttributeNext)
                    {
                        chunk.Append(builder);
                    }
                    else
                    {
                        // TODO: After "attribute support" is baked in, this block needs to move back to... Context.cs?
                        builder.Append("<!-- -->");
                        chunk.Append(builder);
                        builder.Append("<script>r(\"slot");
                        builder.Append(chunk.Id);
                        builder.Append("\")</script>");
                    }
                    break;
                case FormatType.View:
                case FormatType.HtmlString:
                    // Only render extras for HtmlString's trailing sentinel, ignore for the leading sentinel.
                    if (chunk.Id > chunk.Integer)
                    {
                        builder.Append("<script>r(\"slot");
                        builder.Append(chunk.Id);
                        builder.Append("\")</script>");
                    }
                    else
                    {
                        builder.AppendLine();
                    }

                    break;
                default:
                    chunk.Append(builder);
                    break;
            }

            if (chunk.Type == FormatType.StringLiteral && chunk.String?[^1] == '"')
            {
                hackProbablyAnAttributeNext = true;
            }
            else
            {
                hackProbablyAnAttributeNext = false;
            }
        }
    }
}