using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain
{
    public interface IEventMapper
    {
        TU MapToEvent<T, TU>(T source) where TU : IEvent;

        TU MapToCommand<T, TU>(T source) where TU : ICommand;
    }
}