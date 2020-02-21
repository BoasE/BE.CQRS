using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Conventions;

namespace BE.CQRS.Domain.DomainObjects
{
    public sealed class DomainObjectLocator : IDomainObjectLocator
    {
        private static readonly TypeInfo DomainType = typeof(IDomainObject).GetTypeInfo();
        private static readonly TypeInfo Task = typeof(Task).GetTypeInfo();
        private static readonly TypeInfo VoidType = typeof(void).GetTypeInfo();
        private static readonly TypeInfo BehaviorAttributeType = typeof(BehaviorAttribute).GetTypeInfo();
        private static readonly TypeInfo CreateAttributeType = typeof(CreateAttribute).GetTypeInfo();
        private static readonly TypeInfo CreateOrUpdateAttributeType = typeof(CreateOrUpdateAttribute).GetTypeInfo();
        private static readonly TypeInfo UpdateAttributeType = typeof(UpdateAttribute).GetTypeInfo();
        private static readonly TypeInfo UpdateWithoutHistoryAttributeType = typeof(UpdateWithoutHistoryAttribute).GetTypeInfo();
        private static readonly TypeInfo CommandType = typeof(ICommand).GetTypeInfo();

        public IEnumerable<CommandMethodMapping> ResolveConventionalMethods(Type domainObjectType)
        {
            IEnumerable<MethodInfo> methods = domainObjectType
                .GetRuntimeMethods();

            return methods
                .Where(i => !i.IsAbstract && i.IsPublic && HasCommandParameter(i) &&
                    HasConventionalReturnMethod(i) &&
                    IsAnnotated(i))
                .Select(method => ToMapping(domainObjectType, method));
        }

        private static CommandMethodMapping ToMapping(Type domainObjectType, MethodInfo nfo)
        {
            Type type = nfo.GetParameters().First().ParameterType;

            Type attribType = GetBehaviorAttribute(nfo);

            var annotation = attribType.GetTypeInfo();

            CommandMethodMappingKind kind = ResolveCommandMethodKind(annotation);

            bool awaitable = Task.IsAssignableFrom(nfo.ReturnType.GetTypeInfo());

            var result = new CommandMethodMapping(domainObjectType, type, nfo, awaitable, kind);

            return result;
        }

        public IEnumerable<Type> ResolveDomainObjects(params Assembly[] source)
        {
            IEnumerable<Type> types = source
                .SelectMany(i => i.ExportedTypes)
                .Where(IsAccessAbleDomainObject);

            return types;
        }

        private static bool HasConventionalReturnMethod(MethodInfo method)
        {
            TypeInfo returnType = method.ReturnType.GetTypeInfo();

            return VoidType.IsAssignableFrom(returnType) || Task.IsAssignableFrom(returnType);
        }

        private static bool IsAnnotated(MethodInfo method)
        {
            Type type = GetBehaviorAttribute(method);

            return type != null;
        }

        private static Type GetBehaviorAttribute(MethodInfo method)
        {
            return method
                .GetCustomAttributes(true)
                .SingleOrDefault(i => IsBehaviorAttribute(i.GetType()))
                ?
                .GetType();
        }

        private static bool IsBehaviorAttribute(Type type)
        {
            TypeInfo info = type.GetTypeInfo();
            return BehaviorAttributeType.IsAssignableFrom(info);
        }

        private static bool IsAccessAbleDomainObject(Type type)
        {
            TypeInfo info = type.GetTypeInfo();
            return type != null && info.IsPublic && !info.IsAbstract && DomainType.IsAssignableFrom(info);
        }

        private static CommandMethodMappingKind ResolveCommandMethodKind(TypeInfo annotation)
        {
            CommandMethodMappingKind kind;

            if (CreateAttributeType.IsAssignableFrom(annotation))
            {
                kind = CommandMethodMappingKind.Create;
            }
            else if (UpdateWithoutHistoryAttributeType.IsAssignableFrom(annotation))
            {
                kind = CommandMethodMappingKind.UpdateWithoutHistory;
            }
            else if (UpdateAttributeType.IsAssignableFrom(annotation))
            {
                kind = CommandMethodMappingKind.Update;
            }
            else if (CreateOrUpdateAttributeType.IsAssignableFrom(annotation))
            {
                kind = CommandMethodMappingKind.CreateOrUpdate;
            }
            else
            {
                throw new NotSupportedException($"Annotation \"{annotation.Name}\" not supported!");
            }

            return kind;
        }

        private static bool HasCommandParameter(MethodInfo method)
        {
            ParameterInfo[] para = method.GetParameters();

            if (para.Length != 1)
            {
                return false;
            }

            ParameterInfo first = para.First();

            return CommandType.IsAssignableFrom(first.ParameterType.GetTypeInfo());
        }
    }
}