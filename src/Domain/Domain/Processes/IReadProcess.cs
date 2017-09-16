using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.Processes
{
    public interface IReadProcess<in TCommand, TContent> where TCommand : ICommand
    {
        Task<ContentProcessResult<TContent>> ExecuteAsync(TCommand command);
    }
}