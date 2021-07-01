using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.Denormalization;

namespace BE.CQRS.Domain.Events.Handlers
{
    [DebuggerDisplay("{Method.Name} -> {EventTypeInfo.Name}")]
    public sealed class EventHandlerMethod
    {
        private static readonly TypeInfo TaskType = typeof(Task).GetTypeInfo();

        public TypeInfo EventTypeInfo { get; }

        public TypeInfo ParentType { get; }

        public bool Awaitable { get; }

        public MethodInfo Method { get; }

        public bool Background { get;  }

        private EventHandlerMethod(TypeInfo parentType, MethodInfo method, TypeInfo eventTypeInfo, bool awaitable,bool background)
        {
            Awaitable = awaitable;
            EventTypeInfo = eventTypeInfo;
            Method = method;
            ParentType = parentType;
            Background = background;
        }

        public static EventHandlerMethod FromMethod(TypeInfo denormalizerType, MethodInfo nfo)
        {
            bool awaitable = TaskType.IsAssignableFrom(nfo.ReturnType.GetTypeInfo());
            TypeInfo eventType = nfo.GetParameters().Single().ParameterType.GetTypeInfo();

            var attribute = denormalizerType.GetCustomAttribute<DenormalizerAttribute>();
            var isBackground = attribute !=null && attribute.IsBackground;

            return new EventHandlerMethod(denormalizerType, nfo, eventType, awaitable,isBackground);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is EventHandlerMethod && Equals((EventHandlerMethod)obj);
        }

        private bool Equals(EventHandlerMethod other)
        {
            return Equals(EventTypeInfo, other.EventTypeInfo) && Equals(ParentType, other.ParentType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((EventTypeInfo != null ? EventTypeInfo.GetHashCode() : 0) * 397) ^
                    (ParentType != null ? ParentType.GetHashCode() : 0);
            }
        }
    }
}