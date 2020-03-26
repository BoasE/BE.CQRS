using BE.CQRS.Domain;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Events;
using map = AutoMapper;

namespace BE.CQRS.Mapping.AutoMapper
{
    public sealed class AutoObjectMapper : IEventMapper
    {
        private readonly map.IMapper mapper;
        public AutoObjectMapper(map.IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TU MapToEvent<T, TU>(T source) where TU : IEvent
        {
            
            return mapper.Map<T, TU>(source);
        }

        public TU MapToCommand<T, TU>(T source) where TU : ICommand
        {
            return mapper.Map<T, TU>(source);
        }
    }
}