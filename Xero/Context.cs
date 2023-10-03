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
        internal UI<T>.View.Buffer ViewBuffer;
        internal UI<T>.View.Buffer CompareBuffer;
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

        internal void PushMutations(ref UI<T>.View compare)
        {
            if (webSocket == null)
                return;

            var deltas = compare.GetDeltas(ViewBuffer, CompareBuffer);

            StringBuilder? output = null;
            foreach (var delta in deltas)
            {
                if (delta.Type == FormatType.StringLiteral)
                    continue;

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
                    chunk.Action(this);
                else if (chunk.Type == FormatType.ActionAsync)
                    _ = chunk.ActionAsync(this); // Do not await. That'd block this event loop.
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