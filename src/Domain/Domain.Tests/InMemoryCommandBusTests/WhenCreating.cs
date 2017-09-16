using System;
using BE.CQRS.Domain.Commands;
using FakeItEasy;
using Xunit;

namespace BE.CQRS.Domain.Tests.InMemoryCommandBusTests
{
    public class WhenCreating : GivenInMemoryBus
    {
        [Fact]
        public void ItThrowsWhenHandlerIsNull()
        {
            var repo = A.Fake<IDomainObjectRepository>();
            ICommandPipeline pipeline = null;

            Assert.Throws<ArgumentNullException>(() => GetSut(pipeline));
        }
    }
}