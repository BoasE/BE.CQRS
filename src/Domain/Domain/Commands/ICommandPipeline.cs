using System.Threading.Tasks;

namespace BE.CQRS.Domain.Commands
{
    public interface ICommandPipeline
    {
        Task ExecuteAsync(ICommand cmd);
    }
}