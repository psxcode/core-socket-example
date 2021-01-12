using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public class Channel : IDisposable
    {
        bool _isDisposed;

        readonly IMessageDispatcher _dispatcher;
        readonly Stream _stream;
        readonly CancellationTokenSource _cancellationTokenSource;

        Channel(Stream stream, IMessageDispatcher dispatcher)
        {
            _stream = stream;
            _dispatcher = dispatcher;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task SendAsync(string message)
        {
            return Protocol.SendAsync(_stream, Protocol.Encode(message), _cancellationTokenSource.Token);
        }

        void Close()
        {
            _cancellationTokenSource.Cancel();
            _stream.Close();
        }

        public async Task ReceiveLoop()
        {
            while (true) {
                byte[] bytes = await Protocol.ReceiveAsync(_stream, _cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                string? response = await _dispatcher.DispatchAsync(Protocol.Decode(bytes)).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response)) {
                    await SendAsync(response).ConfigureAwait(false);
                }
            }
        }

        ~Channel() => Dispose(false);

        public void Dispose() => Dispose(true);

        void Dispose(bool suppressFinalize)
        {
            if (_isDisposed) {
                return;
            }

            _isDisposed = true;

            // TODO cleanup socket
            Close();
            _stream.Dispose();

            if (suppressFinalize) {
                GC.SuppressFinalize(this);
            }
        }

        public static Channel CreateConnect(
            IPEndPoint endpoint,
            IMessageDispatcher dispatcher
        )
        {
            var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(endpoint);

            return new Channel(
                new NetworkStream(socket, true),
                dispatcher
            );
        }

        public static async Task<Channel> CreateListen(
            IPEndPoint endpoint,
            IMessageDispatcher dispatcher
        )
        {
            var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(endpoint);
            socket.Listen(128);
            
            var clientSocket = await Task.Factory.FromAsync(
                socket.BeginAccept,
                socket.EndAccept,
                null
            ).ConfigureAwait(false);

            return new Channel(
                new NetworkStream(clientSocket, true),
                dispatcher
            );
        }
    }
}