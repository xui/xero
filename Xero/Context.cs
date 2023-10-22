using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Xero;

public abstract partial class UI<T> where T : IViewModel
{
    public class Context
    {
        private UI<T> ui;
        public T ViewModel { get; init; }
        internal WebSocket? webSocket;
        internal HtmlString.Buffer ViewBuffer;
        internal HtmlString.Buffer CompareBuffer;
        private readonly byte[] receiveBuffer = new byte[1024 * 4];
        private readonly byte[] sendBuffer = new byte[1024 * 4];

        public Context(UI<T> ui)
        {
            this.ui = ui;

            ViewBuffer = new();
            CompareBuffer = new();

            // TODO: Need to be more clever about how a new ViewModel is created.
            ViewModel = (T)T.New();
        }

        public HtmlString Compose()
        {
            return HtmlString.Create(ViewBuffer, $"{ui.MainLayout(ViewModel)}");
        }

        public async Task Recompose()
        {
            var compare = HtmlString.Create(CompareBuffer, $"{ui.MainLayout(ViewModel)}");
            var deltas = compare.GetDeltas(ViewBuffer, CompareBuffer);
            await PushMutations(deltas);
        }

        internal async Task WriteResponseAsync(HttpContext httpContext)
        {
            // TODO: Optimize.  No need to convert to a single string when we 
            // have streams and pipes.
            await httpContext.Response.WriteAsync(Compose().ToStringWithExtras());
        }

        internal async Task AssignWebSocket(WebSocketManager webSocketManager)
        {
#if DEBUG
            using (new HotReloadContext<T>(this))
#endif

                // TODO: This is almost correct.  Works across multiple browsers but multiple tabs gets its Action stolen.
                // Rework this once you figure out the various ViewModel state levels.
                ViewModel.OnChanged = async () => await Recompose();

            using (var webSocket = await webSocketManager.AcceptWebSocketAsync())
            {
                this.webSocket = webSocket;
                await Receive(webSocket);
            }
        }

        internal async Task PushMutations(IEnumerable<Chunk>? deltas)
        {
            if (webSocket == null || deltas == null)
                return;

            StringBuilder? output = null;
            foreach (var delta in deltas)
            {
                if (delta.Type == FormatType.StringLiteral)
                {
                    await Push("ws.close();location.reload();");
                    return;
                }

                output ??= new();
                output.Append("slot");
                output.Append(delta.Id);
                output.Append(".nodeValue='");
                delta.Append(output);
                output.Append("';");
            }

            // Swap buffers.
            (CompareBuffer, ViewBuffer) = (ViewBuffer, CompareBuffer);

            if (output is not null)
            {
                // TODO:  Never call ToString()! Change Push() to take in some kind of buffer.
                await Push(output.ToString());
            }
        }

        internal async Task Push(string eval)
        {
            if (webSocket == null || webSocket.State != WebSocketState.Open)
                return;

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
                var chunk = ViewBuffer.chunks.First(c => c.Id == slot);
                switch (chunk.Type)
                {
                    case FormatType.Action:
                        using (this.ViewModel.Batch())
                        {
                            chunk.Action();
                        }
                        break;
                    case FormatType.ActionEvent:
                        using (this.ViewModel.Batch())
                        {
                            var domEvent = ParseEvent(receiveBuffer, index, receiveResult.Count - index);
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