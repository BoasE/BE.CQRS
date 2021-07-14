using System.Collections.Generic;
using System.Reflection;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Logging;
using FakeItEasy;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.ConventionCommandPipelineTests
{
    public class GivenConventionCommandPipeline
    {
        protected ConventionCommandPipeline GetSut()
        {
            var asm = new List<Assembly>() {typeof(FakeObject).GetTypeInfo().Assembly};
            var invoker = A.Fake<IConventionCommandInvoker>();

            return new ConventionCommandPipeline(invoker, new DomainObjectLocator(),new NoopLoggerFactory(), asm);
        }

        [Fact]
        public void ITCreatesTheBindings()
        {
            ConventionCommandPipeline sut = GetSut();
        }
        
        
    }
}