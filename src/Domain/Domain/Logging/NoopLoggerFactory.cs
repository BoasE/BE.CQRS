using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Logging
{
    public sealed class NoopLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new NoopLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
}