using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Xui.Web.Html;

namespace Xui.Web.HttpX;

public abstract partial class UI<T> where T : IViewModel
{
    public class Context
    {
        private readonly UI<T> ui;
        public T ViewModel { get; init; }
        private WebSocket? webSocket;
        private HtmlString? htmlString;
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

        public void Compose()
        {
            htmlString = HtmlString.Create(composition, $"{ui.MainLayout(ViewModel)}");
        }

        internal async Task WriteResponseAsync(HttpContext httpContext)
        {
            // TODO: Optimize.  No need to convert to a single string when we 
            // have streams and pipes.
            Compose();
            if (htmlString is not null) {
                await httpContext.Response.WriteAsync(htmlString.Value.ToStringWithExtras());
            }
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
            if (!IsWebSocketOpen)
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
            if (!IsWebSocketOpen)
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

                if (htmlString is null)
                    continue;

                var (slotId, domEvent) = ParseEvent(receiveBuffer, receiveResult.Count);
                using (this.ViewModel.Batch())
                {
                    htmlString.Value.HandleEvent(slotId, domEvent);
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

        private (int, Event?) ParseEvent(byte[] buffer, int length)
        {
            int i = 0, slot = 0;
            while (true)
            {
                // Convert from ASCII to int, digit by digit.
                int d = buffer[i] - 48;
                if (d >= 0 && d <= 9) {
                    slot = slot * 10 + d;
                    ++i;
                    continue;
                }

                if (i >= length - 1)
                    return (slot, null);
                
                // TODO: Optimize (or hopefully move to SignalR).
                var message = Encoding.UTF8.GetString(buffer, i, length - i);
                var @event = JsonSerializer.Deserialize<Event>(message);
                return (slot, @event);
            }
        }

        public override string ToString()
        {
            return htmlString?.ToString() ?? "(null)";
        }
    }
}