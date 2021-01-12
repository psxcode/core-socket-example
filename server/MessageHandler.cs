using System;
using System.Threading.Tasks;
using Shared;

namespace Client
{
    internal static class MessageHandler
    {
        [Route("/Message[@type='Request' and @action='Heartbeat']")]
        public static Task<HeartbeatResponseMessage> HeartbeatResponseHandler(HeartbeatRequestMessage msg)
        {
            Received(msg);

            var res = new HeartbeatResponseMessage
            {
                Id = msg.Id,
                posData = msg.posData,
                Result = new Result {Status = Status.Success}
            };
            
            Sending(res);

            return Task.FromResult(res);
        }

        static void Received<T>(T msg) where T : Message
        {
            Console.WriteLine($"Received {typeof(T).Name} => Action[ {msg.Action} ] RequestId[ {msg.Id} ]");
        }
        
        static void Sending<T>(T msg) where T : Message
        {
            Console.WriteLine($"Sending {typeof(T).Name} => Action[ {msg.Action} ] RequestId[ {msg.Id} ]");
        }
    }
}