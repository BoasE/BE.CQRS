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

namespace BE.CQRS.Domain.Conventions
{
    public sealed class ConventionCommandInvoker : IConventionCommandInvoker
    {
        private readonly IDomainObjectRepository repository;
        private readonly int retryCount = 10;
        private readonly PolicyAttributeValidator policyValidator = new PolicyAttributeValidator();

        public ConventionCommandInvoker(IDomainObjectRepository repository)
        {
            Precondition.For(() => repository).NotNull();
            this.repository = repository;
        }

        public async Task InvokeAndSaveAsync(Type domainObjectType, ICommand cmd,
            IEnumerable<CommandMethodMapping> commands)
        {
            var currentTry = 0;

            while (currentTry < retryCount)
            {
                AppendResult result = await InvokeAndSaveInternalAsync(domainObjectType, cmd, commands);

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
                    domainObject = repository.New(domainObjectType, cmd.DomainObjectId);
                }
                else
                {
                    domainObject = await repository.Get(cmd.DomainObjectId, domainObjectType);
                }
            }

            if (policyValidator.CheckPolicies(domainObject, cmd, methodMappings.Select(x => x.Method).ToArray()))
            {
                await ApplyCommands(domainObject, cmd, methodMappings);
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
                    var task = method.Method.Invoke(domainObject, new object[] { cmd }) as Task;
                    await task;
                }
                else
                {
                    method.Method.Invoke(domainObject, new object[] { cmd });
                }
            }
        }
    }
}