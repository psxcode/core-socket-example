using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public static class Protocol
    {
        const int HEADER_SIZE = 4;

        public static byte[] Encode(string message) => Encoding.UTF8.GetBytes(message);

        public static string Decode(byte[] body) => Encoding.UTF8.GetString(body);

        public static async Task SendAsync(Stream stream, byte[] bodyBytes, CancellationToken cancellationToken)
        {
            await WriteHeader(stream, bodyBytes.Length);

            await stream.WriteAsync(bodyBytes.AsMemory(0, bodyBytes.Length), cancellationToken).ConfigureAwait(false);
        }

        public static async Task<byte[]> ReceiveAsync(Stream stream, CancellationToken cancellationToken)
        {
            int contentLength = await ReadHeader(stream, cancellationToken);

            return await ReadAsync(stream, contentLength, cancellationToken);
        }

        static async Task<int> ReadHeader(Stream stream, CancellationToken cancellationToken)
        {
            byte[] headerBytes = await ReadAsync(stream, HEADER_SIZE, cancellationToken).ConfigureAwait(false);

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBytes));
        }

        static async Task WriteHeader(Stream stream, int contentLength)
        {
            byte[] headerBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(contentLength));

            await stream.WriteAsync(headerBytes.AsMemory(0, headerBytes.Length)).ConfigureAwait(false);
        }

        static async Task<byte[]> ReadAsync(Stream stream, int bytesToRead, CancellationToken cancellationToken)
        {
            var buffer = new byte[bytesToRead];
            var bytesRead = 0;

            while (bytesRead < bytesToRead) {
                int actualReadBytes = await stream.ReadAsync(buffer.AsMemory(bytesRead, bytesToRead - bytesRead), cancellationToken)
                    .ConfigureAwait(false);

                if (actualReadBytes == 0) {
                    throw new Exception("Socket closed");
                }

                bytesRead += actualReadBytes;
            }

            return buffer;
        }
    }
}