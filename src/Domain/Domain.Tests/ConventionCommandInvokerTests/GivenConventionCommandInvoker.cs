using System;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.Logging;
using FakeItEasy;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.ConventionCommandInvokerTests
{
    public class GivenConventionCommandInvoker
    {
        protected ConventionCommandInvoker GetSut(IDomainObjectRepository repo)
        {
            var sut = new ConventionCommandInvoker(repo, new NoopLoggerFactory());
            return sut;
        }

        [Fact]
        public void ItThrowsWhenRepoIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSut(null));
        }

        [Fact]
        public void Nothing()
        {
            var repo = A.Fake<IDomainObjectRepository>();

            ConventionCommandInvoker sut = GetSut(repo);

            Type type = typeof(FakeObject);
            var cmd = new SampleCommand();
        }
    }
}