using BE.CQRS.Domain;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Events;
using map = AutoMapper;

namespace BE.CQRS.Mapping.AutoMapper
{
    public sealed class AutoObjectMapper : IEventMapper
    {
        public AutoObjectMapper()
        {
        }

        public TU MapToEvent<T, TU>(T source) where TU : IEvent
        {
            return map.Mapper.Map<T, TU>(source);
        }

        public TU MapToCommand<T, TU>(T source) where TU : ICommand
        {
            return map.Mapper.Map<T, TU>(source);
        }
    }
}