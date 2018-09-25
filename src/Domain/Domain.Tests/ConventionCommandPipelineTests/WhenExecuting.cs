using System;
using System.Reflection;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Logging;
using FakeItEasy;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.ConventionCommandPipelineTests
{
    public sealed class WhenExecuting
    {
        private ConventionCommandPipeline GetSut(IDomainObjectRepository repo)
        {
            return ConventionCommandPipeline.CreateDefault(repo, new NoopLoggerFactory(),
                typeof(SampleDomainObject).GetTypeInfo().Assembly);
        }

        [Fact]
        public void ItCallsSave()
        {
            var cmd = new CreateCommandSecond();
            var repo = A.Fake<IDomainObjectRepository>();
            A.CallTo(() => repo.New(A<Type>.Ignored, A<string>.That.IsEqualTo(cmd.DomainObjectId)))
                .ReturnsLazily((Type x) => Activator.CreateInstance(x, "123") as IDomainObject);

            ConventionCommandPipeline sut = GetSut(repo);

            sut.ExecuteAsync(cmd);
        }
    }
}