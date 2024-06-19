using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.Denormalization;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Events.Handlers
{
    public sealed class ConventionEventHandler : IEventHandler
    {
        private static readonly EventHandlerInvoker Invoker = new EventHandlerInvoker();
        private static readonly IDenormalizerLocator Locator = new DenormalizerLocator();
        private static readonly IEventMethodConvetion EventMethodConvetion = new OnPrefixEventMethodConvetion();

        private readonly Denormalizer[] denormalizers;
        private readonly Dictionary<TypeInfo, object> normalizerInstances = new Dictionary<TypeInfo, object>();
        private readonly Func<Type, object> normalizerFactory;
        private readonly EventHandlerRegistry mapping = new EventHandlerRegistry();
        private readonly ILogger logger;

        private readonly IBackgroundEventQueue backgroundEventQueue = null;
        public int HandlerCount => denormalizers.Length;

        public ConventionEventHandler(DenormalizerDiContext diContext, ILogger<ConventionEventHandler> logger, IBackgroundEventQueue backgroundEventQueue = null,
            params Assembly[] projectors) // TODO Unify to one constructor
        {
            Precondition.For(projectors, nameof(projectors))
                .NotNull()
                .True(x => x.Any(), "No projector assemblies were passed!");

            normalizerFactory = diContext.DenormalizerActivator.ResolveDenormalizer;

            IEnumerable<Denormalizer> foundDenormalizers = projectors.SelectMany(i => Locator.DenormalizerFromAsm(i));
            List<Denormalizer> denormalizersWithMethods = ProcessDenormalizerMethods(foundDenormalizers);

            this.backgroundEventQueue = backgroundEventQueue;
            this.logger = logger;
            denormalizers = denormalizersWithMethods.ToArray();
            LogStartInfo();
        }

        public ConventionEventHandler(Func<Type, object> factory, params Assembly[] projectors)
        {
            Precondition.For(projectors, nameof(projectors)).NotNull().True(x => x.Any());

            normalizerFactory = factory;
            IEnumerable<Denormalizer> foundDenormalizers = projectors.SelectMany(i => Locator.DenormalizerFromAsm(i));

            List<Denormalizer> denormalizersWithMethods = ProcessDenormalizerMethods(foundDenormalizers);

            denormalizers = denormalizersWithMethods.ToArray();
            LogStartInfo();
        }

        private void LogStartInfo()
        {
            if (logger != null)
            {
                logger.LogInformation($"Bound {denormalizers.Length} denormalizers and {mapping.Count} methods - {mapping.BackgroundMethodCount} background methods");
            }
        }

        private List<Denormalizer> ProcessDenormalizerMethods(IEnumerable<Denormalizer> foundDenormalizers)
        {
            var denormalizersWithMethods = new List<Denormalizer>();
            foreach (Denormalizer denormalizer in foundDenormalizers)
            {
                EventHandlerMethod[] methods = EventMethodConvetion.ResolveEventMethods(denormalizer.Type).ToArray();

                if (methods.Length <= 0)
                    continue;

                normalizerInstances.Add(denormalizer.Type.GetTypeInfo(), normalizerFactory(denormalizer.Type));

                denormalizersWithMethods.Add(denormalizer);

                mapping.Add(methods);
            }

            return denormalizersWithMethods;
        }

        public async Task HandleAsync(IEvent @event)
        {
            var type = @event.GetType();
            var denormalizerGroups = mapping
                .Resolve(type)
                .GroupBy(x => x.ParentType)
                .ToList();

            var watch = Stopwatch.StartNew();
            foreach (IGrouping<TypeInfo, EventHandlerMethod> denormailzer in denormalizerGroups)
            {
                KeyValuePair<TypeInfo, object> instance =
                    normalizerInstances.Single(x => x.Key.Equals(denormailzer.Key));

                foreach (EventHandlerMethod method in denormailzer)
                {
                    if (method.Background && backgroundEventQueue != null)
                    {
                        var task = SafeInvoke(@event, @method, instance.Value);
                        await backgroundEventQueue.QueueBackgroundWorkItemAsync(x => task);
                    }
                    else
                    {
                        await SafeInvoke(@event, method, instance.Value);
                    }
                }
            }

            watch.Stop();
            logger?.LogTrace("\"{type.FullName}\" handled in {watch.ElapsedMilliseconds}ms", type.FullName,
                watch.ElapsedMilliseconds);
        }

        private async Task SafeInvoke(IEvent @event, EventHandlerMethod method,
            object instance)
        {
            try
            {
                await Invoker.InvokeAsync(@event, method, instance);
            }
            catch (Exception err)
            {
                string msg = $"Error in event handler {err.Message} when handling {@event.GetType().FullName}";

                logger.LogError(err, msg);
            }
        }
    }
}