using System;
using System.Threading.Tasks;

namespace Shared
{
    public interface IMessageDispatcher
    {
        public void Register<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler);
        public void Register<TRequest>(Func<TRequest, Task> handler);
        public Task<string?> DispatchAsync(string message);
    }
}