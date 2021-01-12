using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Client;
using Shared;

namespace Server
{
    internal static class Program
    {
        static async Task Main()
        {
            var dispatcher = new XmlMessageDispatcher();
            dispatcher.Register<HeartbeatRequestMessage, HeartbeatResponseMessage>(MessageHandler.HeartbeatResponseHandler);

            var channel = await Channel.CreateListen(
                new IPEndPoint(IPAddress.Loopback, 3000),
                dispatcher
            );
            
            _ = Task.Run(channel.ReceiveLoop);
            
            Console.WriteLine("Server is running");
            Console.ReadLine();
        }
    }
}