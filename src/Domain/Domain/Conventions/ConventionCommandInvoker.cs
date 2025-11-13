using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Policies;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Conventions
{
    public sealed class ConventionCommandInvoker : IConventionCommandInvoker
    {
        private readonly IDomainObjectRepository repository;
        private readonly int retryCount = 10;
        private readonly RequiresAttributeValidator policyValidator = new RequiresAttributeValidator();
        private readonly ILogger logger;

        public ConventionCommandInvoker(IDomainObjectRepository repository, ILoggerFactory loggerFactory)
        {
            Precondition.For(() => repository).NotNull("Repository has to be set!");
            Precondition.For(() => loggerFactory).NotNull("loggerFactory has to be set!");

            logger = loggerFactory.CreateLogger<ConventionCommandInvoker>();
            this.repository = repository;
        }

        public async Task InvokeAndSaveAsync(Type domainObjectType, ICommand cmd,
            IEnumerable<CommandMethodMapping> commandMapping)
        {
            Precondition.For(domainObjectType, nameof(domainObjectType)).NotNull("Type has to be set!");
            Precondition.For(cmd, nameof(cmd)).IsValidCommand();
            if (commandMapping == null) throw new ArgumentNullException(nameof(commandMapping));

            var mappingList = commandMapping as IList<CommandMethodMapping> ?? new List<CommandMethodMapping>(commandMapping);

            logger.LogTrace("Invoking Command \"{type}\" for \"{domainObjectType}\"", cmd.GetType(), domainObjectType);
            var currentTry = 0;
            AppendResult result = AppendResult.NoUpdate;
            while (currentTry < retryCount)
            {
                if (currentTry > 0)
                {
                    logger.LogTrace("Retrying Command \"{type}\" for \"{domainObjectType}\"...", cmd.GetType(), domainObjectType);
                }

                result = await InvokeAndSaveInternalAsync(domainObjectType, cmd, mappingList);

                if (result.HadWrongVersion)
                {
                    currentTry++;
                }
                else
                {
                    break;
                }
            }

            if (currentTry >= retryCount)
            {
                var msg =
                    $"Saving \"{domainObjectType.Name}\" with id \"{cmd.DomainObjectId}\" failed due to version conflicts";
                logger.LogWarning(msg);
                throw new VersionConflictException(domainObjectType.Name, cmd.DomainObjectId, result.CurrentVersion);
            }
        }

        private async Task<AppendResult> InvokeAndSaveInternalAsync(Type domainObjectType, ICommand cmd,
            IEnumerable<CommandMethodMapping> commands)
        {
            var list = commands as IList<CommandMethodMapping> ?? new List<CommandMethodMapping>(commands);

            bool hasFirst = false;
            CommandMethodMappingKind firstKind = default;
            for (int i = 0; i < list.Count; i++)
            {
                var c = list[i];
                if (!hasFirst)
                {
                    hasFirst = true;
                    firstKind = c.Kind;
                }
                else if (c.Kind != firstKind)
                {
                    throw new NotSupportedException(
                        "Command can only be bound to single kind (update or create) per aggregate");
                }
            }

            if (!hasFirst)
            {
                return AppendResult.NoUpdate;
            }

            bool preventVersionCheck = firstKind == CommandMethodMappingKind.UpdateWithoutHistory ||
                                       firstKind == CommandMethodMappingKind.Create;

            IDomainObject obj = await InvokeAsync(domainObjectType, cmd, firstKind, list);
            return await repository.SaveAsync((dynamic)obj, preventVersionCheck);
        }

        public async Task<IDomainObject> InvokeAsync(Type domainObjectType, ICommand cmd, CommandMethodMappingKind kind,
            IEnumerable<CommandMethodMapping> methodMappings)
        {
            IDomainObject domainObject;

            if (kind == CommandMethodMappingKind.Create)
            {
                domainObject = repository.New(domainObjectType, cmd.DomainObjectId);
            }
            else
            {
                if (kind == CommandMethodMappingKind.UpdateWithoutHistory)
                {
                    logger.LogTrace("Creating new \"{domainObjectType}\" with id {domainObjectId}", domainObjectType,
                        cmd.DomainObjectId);
                    domainObject = repository.New(domainObjectType, cmd.DomainObjectId);
                }
                else
                {
                    logger.LogTrace("Getting existing \"{domainObjectType}\" with id {domainObjectId}",
                        domainObjectType, cmd.DomainObjectId);
                    domainObject = await repository.Get(cmd.DomainObjectId, domainObjectType);
                }
            }

            var list = methodMappings as IList<CommandMethodMapping> ?? new List<CommandMethodMapping>(methodMappings);

            var methodsList = new List<System.Reflection.MethodInfo>(list.Count);
            for (int i = 0; i < list.Count; i++)
                methodsList.Add(list[i].Method);

            if (policyValidator.CheckPolicies(domainObject, cmd, methodsList))
            {
                logger.LogTrace("Applying command on \"{domainObjectType}\" with id {domainObjectId}", domainObjectType,
                    cmd.DomainObjectId);
                await ApplyCommands(domainObject, cmd, list);
            }
            else
            {
                logger.LogTrace("Policy prevented command on \"{domainObjectType}\" with id {domainObjectId}",
                    domainObjectType, cmd.DomainObjectId);
            }

            return domainObject;
        }

        private static async Task ApplyCommands(IDomainObject domainObject, ICommand cmd,
            IList<CommandMethodMapping> group)
        {
            for (int i = 0; i < group.Count; i++)
            {
                var mapping = group[i];
                if (mapping.Awaitable && mapping.AsyncInvoker != null)
                {
                    await mapping.AsyncInvoker(domainObject, cmd);
                }
                else if (mapping.SyncInvoker != null)
                {
                    mapping.SyncInvoker(domainObject, cmd);
                }
                else
                {
                    var task = mapping.Method.Invoke(domainObject, new object[] {cmd}) as Task;
                    if (mapping.Awaitable && task != null)
                        await task;
                }
            }
        }
    }
}