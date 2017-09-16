using System.Threading.Tasks;

namespace BE.CQRS.Domain.Events.Handlers
{
    public interface IEventHandler
    {
        int HandlerCount { get; }

        Task HandleAsync(IEvent @event);
    }
}