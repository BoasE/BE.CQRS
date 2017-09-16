using System;
using System.Reflection;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Commands
{
    public sealed class CommandMethodMapping
    {
        public Type CommandType { get; }

        public MethodInfo Method { get; }

        public Type DomainObjectType { get; }

        public bool Awaitable { get; }

        public CommandMethodMappingKind Kind { get; }

        public CommandMethodMapping(Type domainObjectType, Type commandType, MethodInfo method, bool awaitable,
            CommandMethodMappingKind kind)
        {
            Precondition.For(domainObjectType, nameof(domainObjectType)).NotNull();
            Precondition.For(commandType, nameof(commandType)).NotNull();
            Precondition.For(method, nameof(method)).NotNull();

            DomainObjectType = domainObjectType;
            CommandType = commandType;
            Method = method;
            Kind = kind;
            Awaitable = awaitable;
        }
    }
}