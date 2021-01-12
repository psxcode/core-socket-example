using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Shared;

namespace Client
{
    internal static class Program
    {

        static void Main()
        {
            var dispatcher = new XmlMessageDispatcher();
            
            dispatcher.Register<HeartbeatResponseMessage>(MessageHandler.HeartbeatResponseHandler);
            
            Console.WriteLine("Ready to connect");
            Console.ReadLine();
            
            var channel = Channel.CreateConnect(new IPEndPoint(IPAddress.Loopback, 3000), dispatcher);

            Task.Run(() => HeartbeatLoop(channel, 2));

            Console.ReadLine();
        }

        static async Task HeartbeatLoop(Channel channel, int interval)
        {
            var req = new HeartbeatRequestMessage
            {
                Id = "1",
                posData = new POSData {Id = "POS001"}
            };

            string reqSerialized = XmlSerialization.Serialize<HeartbeatRequestMessage>(req);
            
            while (true) {
                await channel.SendAsync(reqSerialized).ConfigureAwait(false);
                await Task.Delay(1000 * interval);
            }
        }
    }
}