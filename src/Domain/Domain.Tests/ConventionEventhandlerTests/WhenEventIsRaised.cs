using System;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.ConventionEventhandlerTests
{
    public class WhenEventIsRaised : GivenConventionEventHandler
    {
        private readonly ConventionEventHandler sut;

        public WhenEventIsRaised()
        {
            sut = new ConventionEventHandler(Activator.CreateInstance,
                typeof(SampleDenormalizer).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task ItCallsTheMethod()
        {
            IEvent @event = new DenormalizerEvent();

            await sut.HandleAsync(@event);
        }
    }
}