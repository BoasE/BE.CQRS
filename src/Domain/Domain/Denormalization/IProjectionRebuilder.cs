using System.Threading.Tasks;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IProjectionRebuilder
    {
        Task Execute();
    }
}