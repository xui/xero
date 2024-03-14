using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Xui.Web.Tags;

public abstract partial class UI<T> where T : IViewModel
{
    public class Context
    {
        private readonly UI<T> ui;
        public T ViewModel { get; init; }
        private WebSocket? webSocket;
        private Composition composition;
        private Composition compositionCompare;
        private readonly byte[] receiveBuffer = new byte[1024 * 4];
        private readonly byte[] sendBuffer = new byte[1024 * 4];

        private static readonly MemoryCache cache = new(new MemoryCacheOptions());
        private static readonly MemoryCacheEntryOptions entryOptions =
            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));

        public static Context Get(HttpContext httpContext, UI<T> ui)
        {
            var sessionId = httpContext.GetHttpXSessionId();
            if (cache.Get(sessionId) is not Context context)
            {
                context = new Context(ui);
                Set(sessionId, context);
            }
            return context;
        }

        private static void Set(string id, Context context)
        {
            cache.Set(id, context, entryOptions);
        }

        public bool IsWebSocketOpen
        {
            get => webSocket != null && webSocket.State == WebSocketState.Open;
        }

        public Context(UI<T> ui)
        {
            this.ui = ui;

            composition = new();
            compositionCompare = new();

            // TODO: Need to be more clever about how a new ViewModel is created.
            ViewModel = (T)T.New();
        }

        public HtmlString Compose()
        {
            return HtmlString.Create(composition, $"{ui.MainLayout(ViewModel)}");
        }

        internal async Task WriteResponseAsync(HttpContext httpContext)
        {
            // TODO: Optimize.  No need to convert to a single string when we 
            // have streams and pipes.
            await httpContext.Response.WriteAsync(Compose().ToStringWithExtras());
        }

        internal async Task AssignWebSocket(WebSocketManager webSocketManager)
        {
            // TODO: This is almost correct.  Works across multiple browsers but multiple tabs gets its Action stolen.
            // Rework this once you figure out the various ViewModel state levels.
            ViewModel.OnChanged = async () => await Recompose();

#if DEBUG
            using (new HotReloadContext<T>(this))
#endif
            using (var webSocket = await webSocketManager.AcceptWebSocketAsync())
            {
                this.webSocket = webSocket;
                await Receive(webSocket);
            }
        }

        internal async Task Recompose()
        {
            var htmlString = HtmlString.Create(this.compositionCompare, $"{ui.MainLayout(ViewModel)}");
            var deltas = GetDeltas(composition, this.compositionCompare, htmlString.end);
            var ranges = CalculateSlotRanges(deltas);
            if (ranges.Count == 0)
                return;

            await PushMutations(ranges);
        }

        internal IEnumerable<Chunk> GetDeltas(Composition composition, Composition compare, int end)
        {
            for (int i = 0; i < end; i++)
            {
                if (composition.chunks[i] != compare.chunks[i])
                {
                    yield return compare.chunks[i];
                }
            }
        }

        private List<Range> CalculateSlotRanges(IEnumerable<Chunk> deltas)
        {
            List<Range>? ranges = null;
            foreach (var chunk in deltas)
            {
                ranges ??= new();

                if (chunk.Type != FormatType.StringLiteral)
                {
                    ranges.Add(new Range(chunk.Id, chunk.Id));
                }
                else
                {
                    var htmlStringStart = compositionCompare.chunks[chunk.Integer!.Value];
                    ranges.Add(new Range(chunk.Integer.Value, htmlStringStart.Integer!.Value));
                }
            }

            if (ranges == null)
                return Enumerable.Empty<Range>().ToList();

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

            return ranges;
        }

        internal async Task PushMutations(List<Range>? ranges)
        {
            if (webSocket == null || ranges == null || ranges.Count == 0)
                return;

            StringBuilder? output = null;
            foreach (var range in ranges)
            {
                var delta = compositionCompare.chunks[range.End.Value];
                if (range.Start.Value != range.End.Value)
                {
                    // TODO: Optimize.  Any way to cleaning and efficiently trim and escape without calling ToString first?
                    var sb = new StringBuilder();
                    HtmlString.OutputRangeWithExtras(compositionCompare, range.Start.Value, range.End.Value - 1, sb);
                    var content = sb.ToString().Trim();

                    output ??= new();
                    output.Append("replaceNode(slot");
                    output.Append(delta.Id);
                    output.Append(",`");
                    output.Append(content);
                    output.Append("`);");
                }
                else
                {
                    output ??= new();
                    output.Append("slot");
                    output.Append(delta.Id);
                    output.Append(".nodeValue='");
                    delta.Append(output);
                    output.Append("';");
                }
            }

            // Swap buffers.
            (compositionCompare, composition) = (composition, compositionCompare);

            if (output is not null)
            {
                // TODO:  Never call ToString()! Change Push() to take in some kind of buffer.
                await Push(output.ToString());
            }
        }

        internal async Task PushHistoryState(string path)
        {
            await Push($"window.history.pushState({{}},'', '{path}')");
        }

        private async Task Push(string eval)
        {
            if (webSocket == null || webSocket.State != WebSocketState.Open)
                return;

            // eval = eval
            //     .Replace("\"", "\\\"")
            //     .Replace("\n", "");

            // TODO: Optimize.  Skip the string?
            Encoding.Default.GetBytes(eval, 0, eval.Length, sendBuffer, 0);
            await webSocket.SendAsync(
                new ArraySegment<byte>(sendBuffer, 0, eval.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        private async Task Receive(WebSocket webSocket)
        {
            WebSocketReceiveResult? receiveResult;
            do
            {
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(receiveBuffer),
                    CancellationToken.None
                );

                if (receiveResult.Count == 0)
                    continue;

                var (slot, index) = ParseSlotId(receiveBuffer, receiveResult.Count);

                // TODO: Optimize.  Bypass the O(n).  Lazy Dict gets reset on each compose?
                var chunk = composition.chunks.First(c => c.Id == slot);
                switch (chunk.Type)
                {
                    case FormatType.Action:
                        using (this.ViewModel.Batch())
                        {
                            chunk.Action();
                        }
                        break;
                    case FormatType.ActionEvent:
                        var domEvent = ParseEvent(receiveBuffer, index, receiveResult.Count - index);
                        using (this.ViewModel.Batch())
                        {
                            chunk.ActionEvent(domEvent);
                        }
                        break;
                    case FormatType.ActionAsync:
                        // Do not batch.  Mutations should go immediately.
                        // Do not await. That'd block this event loop.
                        _ = chunk.ActionAsync();
                        break;
                    case FormatType.ActionEventAsync:
                        // Do not batch.  Mutations should go immediately.
                        // Do not await. That'd block this event loop.
                        var domEventAsync = ParseEvent(receiveBuffer, index, receiveResult.Count - index);
                        _ = chunk.ActionEventAsync(domEventAsync);
                        break;
                }
            }
            while (!receiveResult.CloseStatus.HasValue);

            Console.WriteLine("Closing the connection...");

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None
            );

            Console.WriteLine("Connection closed.");
        }

        private (int, int) ParseSlotId(byte[] buffer, int length)
        {
            int i = 0, slot = 0;
            while (true)
            {
                int d = buffer[i] - 48;
                if (d >= 0 && d <= 9)
                    slot = slot * 10 + d;
                else
                    return (slot, i);

                if (++i >= length)
                    return (slot, i);
            }
        }

        private Event ParseEvent(byte[] buffer, int index, int length)
        {
            var message = Encoding.UTF8.GetString(buffer, index, length);
            var ev = JsonSerializer.Deserialize<Event>(message);
            return ev ?? new Event();
        }
    }
}