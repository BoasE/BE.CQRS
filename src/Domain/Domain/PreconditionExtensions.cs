using System;
using BE.CQRS.Domain.Commands;
using BE.FluentGuard;

namespace BE.CQRS.Domain
{
    public static class PreconditionExtensions
    {
        public static ValidationRule<ICommand> IsValidCommand(this ValidationRule<ICommand> rule,
            string message = "File must exist")
        {
            rule.NotNull();

            if (string.IsNullOrWhiteSpace(rule.Value.DomainObjectId))
            {
                throw new ArgumentException(message ?? "Command must have a DomainObjectId", rule.Name);
            }

            return rule;
        }
    }
}