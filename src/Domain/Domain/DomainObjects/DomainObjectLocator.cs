using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Conventions;
using BE.FluentGuard;

namespace BE.CQRS.Domain.DomainObjects
{
    public sealed class DomainObjectLocator : IDomainObjectLocator
    {
        private static readonly TypeInfo DomainType = typeof(IDomainObject).GetTypeInfo();
        private static readonly TypeInfo TaskType = typeof(Task).GetTypeInfo();
        private static readonly TypeInfo VoidType = typeof(void).GetTypeInfo();
        private static readonly TypeInfo BehaviorAttributeType = typeof(BehaviorAttribute).GetTypeInfo();
        private static readonly TypeInfo CreateAttributeType = typeof(CreateAttribute).GetTypeInfo();
        private static readonly TypeInfo CommandType = typeof(ICommand).GetTypeInfo();

        public IEnumerable<CommandMethodMapping> ResolveConventionalMethods(Type domainObjectType)
        {
            foreach (var method in domainObjectType.GetRuntimeMethods())
            {
                if (method.IsAbstract || !method.IsPublic)
                    continue;
                if (!HasCommandParameter(method))
                    continue;
                if (!HasConventionalReturnMethod(method))
                    continue;
                var attribType = GetBehaviorAttribute(method);
                if (attribType is null)
                    continue;

                yield return ToMapping(domainObjectType, method, attribType);
            }
        }

        private static CommandMethodMapping ToMapping(Type domainObjectType, MethodInfo nfo, Type attribType)
        {
            var parameters = nfo.GetParameters();
            var paramType = parameters[0].ParameterType;

            var kind = CreateAttributeType.IsAssignableFrom(attribType.GetTypeInfo())
                ? CommandMethodMappingKind.Create
                : CommandMethodMappingKind.Update;

            bool awaitable = TaskType.IsAssignableFrom(nfo.ReturnType.GetTypeInfo());

            return new CommandMethodMapping(domainObjectType, paramType, nfo, awaitable, kind);
        }

        private static bool HasCommandParameter(MethodInfo method)
        {
            var para = method.GetParameters();
            if (para.Length != 1)
                return false;

            var first = para[0];
            return CommandType.IsAssignableFrom(first.ParameterType.GetTypeInfo());
        }

        public IEnumerable<Type> ResolveDomainObjects(IList<Assembly> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            for (int i = 0; i < source.Count; i++)
            {
                var asm = source[i];
                foreach (var t in asm.ExportedTypes)
                {
                    if (IsAccessAbleDomainObject(t))
                        yield return t;
                }
            }
        }

        private static bool HasConventionalReturnMethod(MethodInfo method)
        {
            TypeInfo returnType = method.ReturnType.GetTypeInfo();
            return VoidType.IsAssignableFrom(returnType) || TaskType.IsAssignableFrom(returnType);
        }

        private static Type GetBehaviorAttribute(MethodInfo method)
        {
            // Materialize to list once to avoid multiple enumeration and allow indexing if needed
            var attrs = new List<CustomAttributeData>(method.CustomAttributes);
            for (int i = 0; i < attrs.Count; i++)
            {
                var attrType = attrs[i].AttributeType;
                if (IsBehaviorAttribute(attrType))
                    return attrType;
            }

            return null;
        }

        private static bool IsBehaviorAttribute(Type type)
        {
            TypeInfo info = type.GetTypeInfo();
            return BehaviorAttributeType.IsAssignableFrom(info);
        }

        private static bool IsAccessAbleDomainObject(Type type)
        {
            if (type == null) return false;
            TypeInfo info = type.GetTypeInfo();
            return info.IsPublic && !info.IsAbstract && DomainType.IsAssignableFrom(info);
        }
    }
}