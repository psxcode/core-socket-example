using System;
using System.Threading.Tasks;
using Shared;

namespace Client
{
    internal static class MessageHandler
    {
        [Route("/Message[@type='Response' and @action='Heartbeat']")]
        public static Task HeartbeatResponseHandler(HeartbeatResponseMessage msg)
        {
            Console.WriteLine($"Received {msg.Action}: {msg.Result?.Status}, {msg.Id}");

            return Task.CompletedTask;
        }
    }
}