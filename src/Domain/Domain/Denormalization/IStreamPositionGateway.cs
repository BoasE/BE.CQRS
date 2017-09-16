using System.Threading.Tasks;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IStreamPositionGateway
    {
        Task SaveAsync(string streamName, long pos);

        Task<long?> GetAsync(string streamName);
    }
}