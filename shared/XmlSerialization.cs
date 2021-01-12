using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Shared
{
    public static class XmlSerialization
    {
        static string Serialize(Type T, object instance)
        {
            using var sw = new StringWriter();
            new XmlSerializer(T).Serialize(sw, instance);

            return sw.ToString();
        }

        public static string Serialize<T>(object instance) => Serialize(typeof(T), instance);

        static object Deserialize(Type T, string doc)
        {
            using var sr = new StringReader(doc);

            var instance = new XmlSerializer(T).Deserialize(sr);

            if (instance is null) {
                throw new ArgumentException($"Cannot parse XML document\n{doc}", nameof(doc));
            }

            return instance;
        }

        public static T Deserialize<T>(string doc) => (T) Deserialize(typeof(T), doc);
    }
}