using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Shared
{
    public class XmlMessageDispatcher : IMessageDispatcher
    {
        readonly List<(string xpathExpression, Func<string, Task<string?>> method)> _handlers = new();

        public void Register<TParam, TResult>(Func<TParam, Task<TResult>> handler)
        {
            async Task<string?> Wrapper(string xml)
            {
                var param = XmlSerialization.Deserialize<TParam>(xml);
                var result = await handler(param);

                return result != null
                    ? XmlSerialization.Serialize<TResult>(result)
                    : null;
            }

            string xpath = GetXPathRoute(handler.Method);

            _handlers.Add((xpath, Wrapper));
        }

        public void Register<TParam>(Func<TParam, Task> handler)
        {
            async Task<string?> Wrapper(string xml)
            {
                var param = XmlSerialization.Deserialize<TParam>(xml);
                await handler(param);

                return null;
            }

            string xpath = GetXPathRoute(handler.Method);

            _handlers.Add((xpath, Wrapper));
        }

        public Task<string?> DispatchAsync(string message)
        {
            var doc = XDocument.Parse(message);
            
            foreach (var (xpath, method) in _handlers) {
                if ((doc.XPathEvaluate(xpath) as bool?) == true) {
                    return method(message);
                }
            }

            return Task.FromResult<string?>(null);
        }

        static string GetXPathRoute(MemberInfo info)
        {
            var routeAttr = info.GetCustomAttribute<RouteAttribute>();

            if (routeAttr == null) {
                throw new ArgumentException($"Method {info.Name} does not have Route attribute");
            }

            return $"boolean({routeAttr.Path})";
        }
    }
}