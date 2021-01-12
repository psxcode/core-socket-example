using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shared
{
    public static class JsonSerialization
    {
        static readonly JsonSerializer s_jsonSerializer;

        static JsonSerialization()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = false
                    }
                },
                PreserveReferencesHandling = PreserveReferencesHandling.None
            };

            s_jsonSerializer = JsonSerializer.Create(settings);
        }

        static string Serialize(object instance)
        {
            using var sw = new StringWriter();
            s_jsonSerializer.Serialize(sw, instance);

            return sw.ToString();
        }

        static object Deserialize(string doc)
        {
            using var sr = new StringReader(doc);
            using var jr = new JsonTextReader(sr);

            var instance = s_jsonSerializer.Deserialize(jr);

            if (instance is null) {
                throw new ArgumentException($"Cannot parse JSON document\n{doc}", nameof(doc));
            }

            return instance;
        }

        static T Deserialize<T>(string doc) => (T) Deserialize(doc);
    }
}