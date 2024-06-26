﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Commands
{
    public sealed class InMemoryCommandBus : CommandBusBase
    {
        private readonly Subject<ICommand> subject = new Subject<ICommand>();
        private readonly ICommandPipeline handler;
        private readonly ILogger logger;

        public InMemoryCommandBus(ICommandPipeline handler, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Precondition.For(() => handler).NotNull();
            logger = loggerFactory.CreateLogger<InMemoryCommandBus>();
            this.handler = handler;

            subject.Subscribe(async msg => await HandleCommand(msg));
        }

        private async Task<CommandBusResult> HandleCommand(ICommand cmd)
        {
            var type = cmd.GetType();

            CommandBusResult result;
            try
            {
                logger.LogTrace("Handling command \"{type}\" \"{id}\"", type, cmd.DomainObjectId);
                await handler.ExecuteAsync(cmd);
                result = CommandBusResult.Succeeded();
            }
            catch (Exception err)
            {
                LogError(err, type);

                result = CommandBusResult.Failed(err);
            }

            return result;
        }
        private void LogError(Exception err, Type type)
        {
            var msg = err.Message;
            logger.LogError(err, "Error handling command \"{type}\" - {msg}", type, msg);
        }

        protected override async Task<CommandBusResult> EnqueueInternalAsync(ICommand cmd)
        {
            Precondition.For(cmd, nameof(cmd)).IsValidCommand();
            CommandBusResult result = await HandleCommand(cmd);
            return result;
        }

        public static InMemoryCommandBus CreateConventionCommandBus(IDomainObjectRepository repository,
            ILoggerFactory loggerFactory,
            EventSourceConfiguration configuration)
        {
            Precondition.For(repository, nameof(repository)).NotNull();
            Precondition.For(loggerFactory, nameof(loggerFactory)).NotNull();
            Precondition.For(configuration, nameof(configuration)).NotNull();

            var logger = loggerFactory.CreateLogger<InMemoryCommandBus>();
            logger.LogInformation("Building InMemory CommandBus with convention based pipeline");

            var invoker = new ConventionCommandInvoker(repository, loggerFactory);
            var handler = new ConventionCommandPipeline(invoker, new DomainObjectLocator(), loggerFactory,
                configuration.DomainObjectAssemblies);

            var bus = new InMemoryCommandBus(handler, loggerFactory);

            return bus;
        }
    }
}