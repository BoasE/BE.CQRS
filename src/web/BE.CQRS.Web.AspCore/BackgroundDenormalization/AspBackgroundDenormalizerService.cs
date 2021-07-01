using System;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.Denormalization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspCore.BackgroundDenormalization
{
    public sealed class AspBackgroundDenormalizerService : BackgroundService
    {
        private readonly ILogger<AspBackgroundDenormalizerService> _logger;

        public AspBackgroundDenormalizerService(IBackgroundEventQueue taskQueue,
            ILogger<AspBackgroundDenormalizerService> logger)
        {
            TaskQueue = taskQueue;
            _logger = logger;
        }

        public IBackgroundEventQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}