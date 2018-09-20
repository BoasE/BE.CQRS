using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
            Precondition.For(commandMapping, nameof(commandMapping)).NotNull();

            var type = cmd.GetType();
            
            logger.LogTrace("Invoking Command \"{type}\" for \"{domainObjectType}\"",domainObjectType);
            var currentTry = 0;

            while (currentTry < retryCount)
            {
                if (currentTry > 0)
                {
                    logger.LogTrace("Retrying Command \"{type}\" for \"{domainObjectType}\"...");
                }
                AppendResult result = await InvokeAndSaveInternalAsync(domainObjectType, cmd, commandMapping);

                if (result.HadWrongVersion)
                {
                    
                    currentTry++;
                }
                else
                {
                    break;
                }
            }
        }

        private async Task<AppendResult> InvokeAndSaveInternalAsync(Type domainObjectType, ICommand cmd,
            IEnumerable<CommandMethodMapping> commands)
        {
            CommandMethodMappingKind[] kinds = commands.Select(i => i.Kind).Distinct().ToArray();
            if (kinds.Length != 1)
            {
                throw new NotSupportedException(
                    "Command can only be bound to single kind (update or create) per aggregate");
            }

            bool preventVersionCheck = kinds[0] == CommandMethodMappingKind.UpdateWithoutHistory ||
                                       kinds[0] == CommandMethodMappingKind.Create;

            IDomainObject obj = await InvokeAsync(domainObjectType, cmd, kinds.First(), commands.ToArray());
            return await repository.SaveAsync(obj, preventVersionCheck);
        }

        public async Task<IDomainObject> InvokeAsync(Type domainObjectType, ICommand cmd, CommandMethodMappingKind kind,
            ICollection<CommandMethodMapping> methodMappings)
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
                    logger.LogTrace("Creating new \"{domainObjectType}\" with id {domainObjectId}",domainObjectType,cmd.DomainObjectId);
                    domainObject = repository.New(domainObjectType, cmd.DomainObjectId);
                }
                else
                {
                    logger.LogTrace("Getting existing \"{domainObjectType}\" with id {domainObjectId}",domainObjectType,cmd.DomainObjectId);
                    domainObject = await repository.Get(cmd.DomainObjectId, domainObjectType);
                }
            }

            if (policyValidator.CheckPolicies(domainObject, cmd, methodMappings.Select(x => x.Method).ToArray()))
            {
                logger.LogTrace("Applying command on \"{domainObjectType}\" with id {domainObjectId}",domainObjectType,cmd.DomainObjectId);
                await ApplyCommands(domainObject, cmd, methodMappings);
            }
            else
            {
                logger.LogTrace("Policy prevented command on \"{domainObjectType}\" with id {domainObjectId}",domainObjectType,cmd.DomainObjectId);
            }

            return domainObject;
        }

        private static async Task ApplyCommands(IDomainObject domainObject, ICommand cmd,
            IEnumerable<CommandMethodMapping> group)
        {
            foreach (CommandMethodMapping method in group)
            {
                if (method.Awaitable)
                {
                    var task = method.Method.Invoke(domainObject, new object[] {cmd}) as Task;
                    await task;
                }
                else
                {
                    method.Method.Invoke(domainObject, new object[] {cmd});
                }
            }
        }
    }
}