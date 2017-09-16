using System.Threading.Tasks;

namespace BE.CQRS.Domain.Commands
{
    public interface ICommandBus
    {
        Task<CommandBusResult> EnqueueAsync(ICommand cmd);
    }
}