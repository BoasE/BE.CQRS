using System.Reflection;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;
using FakeItEasy;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.ConventionCommandPipelineTests
{
    public class GivenConventionCommandPipeline
    {
        protected ConventionCommandPipeline GetSut()
        {
            Assembly asm = typeof(FakeObject).GetTypeInfo().Assembly;
            var invoker = A.Fake<IConventionCommandInvoker>();

            return new ConventionCommandPipeline(invoker, new DomainObjectLocator(), asm);
        }

        [Fact]
        public void ITCreatesTheBindings()
        {
            ConventionCommandPipeline sut = GetSut();
        }
    }
}