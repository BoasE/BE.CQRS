using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.Processes
{
    public interface IProcess<in TCommand> where TCommand : ICommand
    {
        Task<ProcessResult> ExecuteAsync(TCommand command);
    }
}