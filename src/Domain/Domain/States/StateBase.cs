using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;
using BE.FluentGuard;

namespace BE.CQRS.Domain.States
{
    public abstract class StateBase : IState
    {
        private static readonly IEventMethodConvetion MethodConvetion = new EventMethodConvetion();
        private static readonly EventHandlerInvoker Invoker = new EventHandlerInvoker();

        private bool breakRequested;

        private readonly IEventHandlerRegistry registry;

        protected StateBase()
        {
            EventHandlerMethod[] methods = MethodConvetion.ResolveEventMethods(GetType()).ToArray();
            registry = new EventHandlerRegistry();
            registry.Add(methods);
        }

        public void Execute(IEnumerable<IEvent> source)
        {
            Precondition.For(source, nameof(source)).NotNull();

            foreach (IEvent entry in source)
            {
                On(entry);

                if (breakRequested)
                {
                    break;
                }
            }
        }

        protected void Break()
        {
            breakRequested = true;
        }

        protected virtual void On(IEvent @event)
        {
            InvokeMethods(@event).Wait();
        }

        private async Task InvokeMethods(IEvent @event)
        {
            EventHandlerMethod[] methods = registry.Resolve(@event.GetType());

            foreach (EventHandlerMethod method in methods)
            {
                await Invoker.InvokeAsync(@event, method, this); //Todo get events    
            }
        }
    }
}