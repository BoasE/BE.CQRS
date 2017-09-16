using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BE.CQRS.Domain.Events.Handlers
{
    public sealed class EventMethodConvetion : IEventMethodConvetion
    {
        private static readonly TypeInfo EventType = typeof(IEvent).GetTypeInfo();

        public IEnumerable<EventHandlerMethod> ResolveEventMethods(Type type)
        {
            TypeInfo nfo = type.GetTypeInfo();

            return type.GetRuntimeMethods()
                .Where(i => !i.IsAbstract && i.IsPublic && !i.IsConstructor && !i.IsStatic &&
                    MatchesNameConvention(i) &&
                    HasParameterSignature(i))
                .Select(i => EventHandlerMethod.FromMethod(nfo, i));
        }

        private static bool MatchesNameConvention(MemberInfo method)
        {
            return method.Name.Equals("on", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasParameterSignature(MethodBase mthd)
        {
            ParameterInfo[] parametrs = mthd.GetParameters();

            if (parametrs.Length != 1)
            {
                return false;
            }

            return EventType.IsAssignableFrom(parametrs.First().ParameterType.GetTypeInfo());
        }
    }
}