using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Xero;

public abstract partial class UI<T> where T : IViewModel
{
    public class Context
    {
        public T ViewModel { get; init; }
        public ClaimsPrincipal? User { get; set; }

        internal WebSocket? webSocket;
        internal HtmlString.Buffer ViewBuffer;
        internal HtmlString.Buffer CompareBuffer;
        private readonly byte[] receiveBuffer = new byte[1024 * 4];
        private readonly byte[] sendBuffer = new byte[1024 * 4];

        public Context()
        {
            ViewBuffer = new();
            CompareBuffer = new();
            ViewModel = (T)T.New();
        }

        internal async Task AssignWebSocket(WebSocketManager webSocketManager)
        {
            using (var webSocket = await webSocketManager.AcceptWebSocketAsync())
            {
                this.webSocket = webSocket;
                await Receive(webSocket);
            }
        }

        internal void PushMutations(ref HtmlString compare)
        {
            if (webSocket == null)
                return;

            var deltas = compare.GetDeltas(ViewBuffer, CompareBuffer);

            StringBuilder? output = null;
            foreach (var delta in deltas)
            {
                if (delta.Type == FormatType.StringLiteral)
                {
                    Task.Run(async () =>
                    {
                        await Push("ws.close();location.reload();");
                    });
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
                Task.Run(async () =>
                {
                    // TODO:  Never call ToString()! Change Push() to take in some kind of buffer.
                    await Push(output.ToString());
                });
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

                // TODO: Optimize.  Skip the string.  Go straight from buffer to int.
                var message = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                int slot = int.Parse(message);

                // TODO: Optimize.  Bypass the O(n).  Lazy Dict gets reset on each compose?
                var chunk = ViewBuffer.chunks.First(c => c.Id == slot);
                if (chunk.Type == FormatType.Action)
                {
                    using (this.ViewModel.Batch())
                    {
                        chunk.Action();
                    }
                }
                else if (chunk.Type == FormatType.ActionAsync)
                {
                    // Do not batch.  Mutations should go immediately.
                    // Do not await. That'd block this event loop.
                    _ = chunk.ActionAsync();
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
    }
}