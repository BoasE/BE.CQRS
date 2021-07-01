using System;
using System.Threading;
using System.Threading.Tasks;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IBackgroundEventQueue
    {
        Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}