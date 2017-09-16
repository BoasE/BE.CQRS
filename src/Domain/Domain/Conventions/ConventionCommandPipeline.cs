using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;

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

        public static ConventionCommandPipeline CreateDefault(IDomainObjectRepository repo, params Assembly[] asm)
        {
            return new ConventionCommandPipeline(new ConventionCommandInvoker(repo), new DomainObjectLocator(), asm);
        }

        public ConventionCommandPipeline(IConventionCommandInvoker invoker, IDomainObjectLocator locator,
            params Assembly[] domainObjectAssemblies)
        {
            this.locator = locator;
            this.invoker = invoker;
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

            Trace.WriteLine($"Handled {count} domainobjects from {domainObjectAssemblies.Length} assemblies", Category);
        }

        public Task ExecuteAsync(ICommand cmd)
        {
            Type type = cmd.GetType();

            Trace.WriteLine($"Executing \"{type.FullName}\"", Category);

            List<CommandMethodMapping> mapping = resolvedMappings.GetOrAdd(type, ResolveMappings);

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