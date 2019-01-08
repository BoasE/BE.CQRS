using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Logging
{
    public sealed class ConsoleLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
}