using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Denormalization;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Events.Handlers
{
    public sealed class ConventionEventHandler : IEventHandler
    {
        private static readonly EventHandlerInvoker Invoker = new EventHandlerInvoker();
        private static readonly IDenormalizerLocator Locator = new DenormalizerLocator();
        private static readonly IEventMethodConvetion EventMethodConvetion = new EventMethodConvetion();

        private readonly Type[] denormalizers;
        private readonly Dictionary<TypeInfo, object> normalizerInstances = new Dictionary<TypeInfo, object>();
        private readonly Func<Type, object> normalizerFactory;
        private readonly EventHandlerRegistry mapping = new EventHandlerRegistry();

        public int HandlerCount => denormalizers.Length;

        public ConventionEventHandler(IDenormalizerActivator activator, params Assembly[] projectors) // TODO Unify to one constructor (Breaking changes to geteventstore possible)
        {
            Precondition.For(projectors, nameof(projectors)).NotNull().True(x => x.Any());

            normalizerFactory = activator.ResolveDenormalizer;
            IEnumerable<Type> foundDenormalizers = projectors.SelectMany(i => Locator.DenormalizerFromAsm(i));

            List<Type> denormalizersWithMethods = ProcessDenormalizerMethods(foundDenormalizers);

            denormalizers = denormalizersWithMethods.ToArray();
        }

        public ConventionEventHandler(Func<Type, object> factory, params Assembly[] projectors)
        {
            Precondition.For(projectors, nameof(projectors)).NotNull().True(x => x.Any());

            normalizerFactory = factory;
            IEnumerable<Type> foundDenormalizers = projectors.SelectMany(i => Locator.DenormalizerFromAsm(i));

            List<Type> denormalizersWithMethods = ProcessDenormalizerMethods(foundDenormalizers);

            denormalizers = denormalizersWithMethods.ToArray();
        }

        private List<Type> ProcessDenormalizerMethods(IEnumerable<Type> foundDenormalizers)
        {
            var denormalizersWithMethods = new List<Type>();
            foreach (Type denormalizer in foundDenormalizers)
            {
                EventHandlerMethod[] methods = EventMethodConvetion.ResolveEventMethods(denormalizer).ToArray();

                if (methods.Length <= 0)
                    continue;

                normalizerInstances.Add(denormalizer.GetTypeInfo(), normalizerFactory(denormalizer));

                denormalizersWithMethods.Add(denormalizer);

                mapping.Add(methods);
            }
            return denormalizersWithMethods;
        }

        public async Task HandleAsync(IEvent @event)
        {
            Type type = @event.GetType();
            List<IGrouping<TypeInfo, EventHandlerMethod>> denormalizerGroups = mapping.Resolve(type).GroupBy(x => x.ParentType).ToList();

            Stopwatch watch = Stopwatch.StartNew();
            foreach (IGrouping<TypeInfo, EventHandlerMethod> denormailzer in denormalizerGroups)
            {
                KeyValuePair<TypeInfo, object> instance = normalizerInstances.Single(x => x.Key.Equals(denormailzer.Key));

                foreach (EventHandlerMethod method in denormailzer)
                {
                    await SafeInvoke(@event, method, instance.Value);
                }
            }
            watch.Stop();
            Trace.WriteLine($"\"{type.FullName}\" handled in {watch.ElapsedMilliseconds}ms", "BE.CQRS.EventHandler");
        }

        internal static async Task SafeInvoke(IEvent @event, EventHandlerMethod method,
            object instance)
        {
            try
            {
                await Invoker.InvokeAsync(@event, method, instance);
            }
            catch (Exception err)
            {
                string msg = $"Error in event handler {err.Message} when handling {@event.GetType().FullName}";

                Trace.WriteLine(msg, "BE.CQRS.EventHandler");
            }
        }
    }
}