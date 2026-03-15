using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Conventions
{
    public sealed class ConventionCommandPipeline : ICommandPipeline
    {
        private readonly IDomainObjectLocator locator;
        private IReadOnlyDictionary<Type, List<CommandMethodMapping>> commandMapping;

        private readonly ConcurrentDictionary<Type, List<CommandMethodMapping>> resolvedMappings = new();

        private readonly IConventionCommandInvoker invoker;

        private readonly ILogger logger;

        public static ConventionCommandPipeline CreateDefault(IDomainObjectRepository repo,
            ILoggerFactory loggerFactory, params Assembly[] asm)
        {
            return new ConventionCommandPipeline(new ConventionCommandInvoker(repo, loggerFactory),
                new DomainObjectLocator(),
                loggerFactory, asm);
        }

        public ConventionCommandPipeline(IConventionCommandInvoker invoker, IDomainObjectLocator locator,
            ILoggerFactory loggerFactory,
            IEnumerable<Assembly> domainObjectAssemblies)
        {
            this.locator = locator;
            this.invoker = invoker;
            logger = loggerFactory.CreateLogger<ConventionCommandPipeline>();
            BindDomainObjects(domainObjectAssemblies is ISet<Assembly> list
                ? list
                : new HashSet<Assembly>(domainObjectAssemblies));
        }

        private void BindDomainObjects(ISet<Assembly> domainObjectAssemblies)
        {
            var types = locator.ResolveDomainObjects(domainObjectAssemblies).ToList();

            var count = 0;

            var commandMappings = new Dictionary<Type, List<CommandMethodMapping>>();

            foreach (Type domainObjectType in types)
            {
                count++;

                IEnumerable<CommandMethodMapping> methodsOfType =
                    locator.ResolveConventionalMethods(domainObjectType).ToList();

                foreach (CommandMethodMapping method in methodsOfType)
                {
                    if (!commandMappings.TryGetValue(method.CommandType, out var list))
                    {
                        list = new List<CommandMethodMapping>();
                        commandMappings.Add(method.CommandType, list);
                    }

                    list.Add(method);
                }
            }

            commandMapping = commandMappings;

            var assemblyCount = domainObjectAssemblies.Count;
            logger.LogInformation("Found {count} DomainObjects in {assemblyCount} assemblies", count, assemblyCount);
        }

        public Task ExecuteAsync(ICommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).IsValidCommand();

            Type type = cmd.GetType();

            List<CommandMethodMapping> mapping = resolvedMappings.GetOrAdd(type, ResolveMappings);

            int mappingCount = mapping.Count;
            logger.LogTrace("Executing command \"{type}\" for {mappingCount} recievers", type, mappingCount);

            // Group by DomainObjectType ohne LINQ
            var groups = new Dictionary<Type, List<CommandMethodMapping>>();
            foreach (var m in mapping)
            {
                if (!groups.TryGetValue(m.DomainObjectType, out var list))
                {
                    list = new List<CommandMethodMapping>();
                    groups.Add(m.DomainObjectType, list);
                }

                list.Add(m);
            }

            var tasks = new List<Task>(groups.Count);
            foreach (var kvp in groups)
            {
                tasks.Add(invoker.InvokeAndSaveAsync(kvp.Key, cmd, kvp.Value));
            }

            return Task.WhenAll(tasks);
        }

        private List<CommandMethodMapping> ResolveMappings(Type commandType)
        {
            TypeInfo nfo = commandType.GetTypeInfo();

            var result = new List<CommandMethodMapping>();
            foreach (var kvp in commandMapping)
            {
                if (nfo.IsAssignableFrom(kvp.Key.GetTypeInfo()))
                {
                    // Add all target mappings
                    var list = kvp.Value;
                    for (int i = 0; i < list.Count; i++)
                        result.Add(list[i]);
                }
            }

            return result;
        }
    }
}