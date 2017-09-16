using System;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Commands
{
    public sealed class CommandBusResult
    {
        public bool WasSuccessfull { get; }

        public bool WasExecuted { get; }

        public bool WasFiltered { get; }

        public Exception Exception { get; }

        private CommandBusResult(bool success, bool executed, bool filtered, Exception exception)
        {
            WasSuccessfull = success;
            Exception = exception;
            WasExecuted = executed;
            WasFiltered = filtered;
        }

        public static CommandBusResult Failed(Exception err)
        {
            Precondition.For(err, nameof(err)).NotNull();

            return new CommandBusResult(success: false, executed: false, filtered: false, exception: err);
        }

        public static CommandBusResult Succeeded()
        {
            return new CommandBusResult(success: true, executed: false, filtered: false, exception: null);
        }

        public static CommandBusResult Filtered()
        {
            return new CommandBusResult(success: false, executed: false, filtered: true, exception: null);
        }
    }
}