using System;
using System.Reflection;
using System.Threading.Tasks;
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

        // Cached delegates to invoke the method without MethodInfo.Invoke allocations
        internal Func<object, object, Task> AsyncInvoker { get; }
        internal Action<object, object> SyncInvoker { get; }

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

            // Build invokers once
            if (awaitable)
            {
                AsyncInvoker = BuildAsyncInvoker(method, domainObjectType, commandType);
            }
            else
            {
                SyncInvoker = BuildSyncInvoker(method, domainObjectType, commandType);
            }
        }

        private static Func<object, object, Task> BuildAsyncInvoker(MethodInfo method, Type targetType, Type commandType)
        {
            // (object target, object cmd) => (Task)((TTarget)target).Method((TCmd)cmd)
            var targetParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "target");
            var cmdParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "cmd");

            var castTarget = System.Linq.Expressions.Expression.Convert(targetParam, targetType);
            var castCmd = System.Linq.Expressions.Expression.Convert(cmdParam, commandType);
            var call = System.Linq.Expressions.Expression.Call(castTarget, method, castCmd);
            var asTask = System.Linq.Expressions.Expression.Convert(call, typeof(Task));

            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object, Task>>(asTask, targetParam, cmdParam);
            return lambda.Compile();
        }

        private static Action<object, object> BuildSyncInvoker(MethodInfo method, Type targetType, Type commandType)
        {
            // (object target, object cmd) => ((TTarget)target).Method((TCmd)cmd)
            var targetParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "target");
            var cmdParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "cmd");

            var castTarget = System.Linq.Expressions.Expression.Convert(targetParam, targetType);
            var castCmd = System.Linq.Expressions.Expression.Convert(cmdParam, commandType);
            var call = System.Linq.Expressions.Expression.Call(castTarget, method, castCmd);

            var lambda = System.Linq.Expressions.Expression.Lambda<Action<object, object>>(call, targetParam, cmdParam);
            return lambda.Compile();
        }
    }
}