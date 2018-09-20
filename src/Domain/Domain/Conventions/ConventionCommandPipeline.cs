using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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

        private readonly ConcurrentDictionary<Type, List<CommandMethodMapping>> resolvedMappings =
            new ConcurrentDictionary<Type, List<CommandMethodMapping>>();

        private readonly IConventionCommandInvoker invoker;

        private static readonly string Category = typeof(ConventionCommandPipeline).FullName;

        private readonly ILogger logger;

        public static ConventionCommandPipeline CreateDefault(IDomainObjectRepository repo,
            ILoggerFactory loggerFactory, params Assembly[] asm)
        {
            return new ConventionCommandPipeline(new ConventionCommandInvoker(repo), new DomainObjectLocator(),
                loggerFactory, asm);
        }

        public ConventionCommandPipeline(IConventionCommandInvoker invoker, IDomainObjectLocator locator,
            ILoggerFactory loggerFactory,
            params Assembly[] domainObjectAssemblies)
        {
            this.locator = locator;
            this.invoker = invoker;
            logger = loggerFactory.CreateLogger<ConventionCommandPipeline>();
            BindDomainObjects(domainObjectAssemblies);
        }

        private void BindDomainObjects(Assembly[] domainObjectAssemblies)
        {
            IEnumerable<Type> types = locator.ResolveDomainObjects(domainObjectAssemblies);

            var count = 0;

            var commandMappings = new Dictionary<Type, List<CommandMethodMapping>>();

            foreach (Type domainObjectType in types)
            {
                count++;

                IEnumerable<CommandMethodMapping> methodsOfType = locator.ResolveConventionalMethods(domainObjectType);

                foreach (CommandMethodMapping method in methodsOfType)
                {
                    if (!commandMappings.ContainsKey(method.CommandType))
                    {
                        commandMappings.Add(method.CommandType, new List<CommandMethodMapping>());
                    }

                    commandMappings[method.CommandType].Add(method);
                }
            }

            commandMapping = commandMappings;

            var assemblyCount = domainObjectAssemblies.Length;
            logger.LogDebug("Found {count} DomainObjects in {assemblyCount} assemblies", count, assemblyCount);
        }

        public Task ExecuteAsync(ICommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).IsValidCommand();

            Type type = cmd.GetType();


            List<CommandMethodMapping> mapping = resolvedMappings.GetOrAdd(type, ResolveMappings);

            int mappingCount = mapping.Count;
            logger.LogTrace("Executing command \"{type}\" for {mappingCount} recievers", type,mappingCount);

            IEnumerable<IGrouping<Type, CommandMethodMapping>> groups = mapping.GroupBy(i => i.DomainObjectType);

            List<Task> tasks = groups.Select(group => invoker.InvokeAndSaveAsync(group.Key, cmd, group)).ToList();

            return Task.WhenAll(tasks);
        }

        private List<CommandMethodMapping> ResolveMappings(Type commandType)
        {
            TypeInfo nfo = commandType.GetTypeInfo();

            List<CommandMethodMapping> result = commandMapping.Where(i => nfo.IsAssignableFrom(i.Key.GetTypeInfo()))
                .SelectMany(i => i.Value)
                .ToList();
            return result;
        }
    }
}