using System;
using System.Threading.Tasks;

namespace BE.CQRS.Domain.Events.Handlers
{
    internal sealed class EventHandlerInvoker
    {
        internal Task InvokeAsync(IEvent @event, EventHandlerMethod method,
            object instance)
        {
            object result = method.Method.Invoke(instance, new object[] { @event });

            if (!method.Awaitable)
                return Task.FromResult(true);

            if (!(result is Task task))
            {
                throw new InvalidOperationException("Expected return type of \"Task\"!");
            }

            return task;
        }
    }
}