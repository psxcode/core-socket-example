using System.Xml.Serialization;

namespace Shared
{
    public enum MessageType
    {
        Request,
        Response
    }

    public enum Status
    {
        Success,
        Failure,
    }

    [XmlRoot("Message")]
    public abstract class Message
    {
        [XmlAttribute("id")]
        public string? Id { get; set; }
        
        [XmlAttribute("type")]
        public MessageType Type { get; set; }
        
        [XmlAttribute("action")]
        public string? Action { get; set; }
    }

    public class POSData
    {
        [XmlAttribute("id")]
        public string? Id { get; set; }
    }

    public class Result
    {
        [XmlAttribute("status")]
        public Status Status { get; set; }
    }

    [XmlRoot("Message")]
    public class HeartbeatRequestMessage : Message
    {
        [XmlElement("POS")]
        public POSData? posData { get; set; }

        public HeartbeatRequestMessage()
        {
            Type = MessageType.Request;
            Action = "Heartbeat";
        }
    }
    
    [XmlRoot("Message")]
    public class HeartbeatResponseMessage : Message
    {
        [XmlElement("Result")]
        public Result? Result { get; set; }
        
        [XmlElement("POS")]
        public POSData? posData { get; set; }

        public HeartbeatResponseMessage()
        {
            Type = MessageType.Response;
            Action = "Heartbeat";
        }
    }
}