using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Logging
{
    public sealed class ConsoleLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var msg = formatter(state, exception);
            Console.WriteLine(msg);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return Disposable.Empty;
        }
    }
}