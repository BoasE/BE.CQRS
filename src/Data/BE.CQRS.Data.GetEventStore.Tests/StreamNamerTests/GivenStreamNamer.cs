using System;
using BE.CQRS.Data.GetEventStore.Tests.RepositoryTests;
using BE.CQRS.Domain.DomainObjects;
using Xunit;

namespace BE.CQRS.Data.GetEventStore.Tests.StreamNamerTests
{
    public class GivenStreamNamer
    {
        private readonly SampleDomainObject sampleObject = new SampleDomainObject("1");
        private const string TestPrefix = nameof(GivenStreamNamer) + ".";

        private const string repoPrefix = "Abc";

        protected StreamTypeNamer GetSut()
        {
            return new StreamTypeNamer(repoPrefix);
        }

        [Fact(DisplayName = TestPrefix + nameof(ItThrownWheStringIdIsNull))]
        public void ItThrownWheStringIdIsNull()
        {
            StreamTypeNamer sut = GetSut();
            string id = null;
            Assert.Throws<ArgumentNullException>(() => sut.Resolve(typeof(IDomainObject), id));
        }

        [Fact(DisplayName = TestPrefix + nameof(ItThrownWhenDomainObjectNull))]
        public void ItThrownWhenDomainObjectNull()
        {
            StreamTypeNamer sut = GetSut();
            Assert.Throws<ArgumentNullException>(() => sut.Resolve((IDomainObject)null));
        }

        [Fact(DisplayName = TestPrefix + nameof(ItContainesTheClassName))]
        public void ItContainesTheClassName()
        {
            string result = ExecuteResolve();

            Assert.True(result.IndexOf(nameof(SampleDomainObject), StringComparison.OrdinalIgnoreCase) > 0);
        }

        [Fact(DisplayName = TestPrefix + nameof(ItContainsThePrefix))]
        public void ItContainsThePrefix()
        {
            string result = ExecuteResolve();

            Assert.True(result.StartsWith(repoPrefix, StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = TestPrefix + nameof(ItContainsTheId))]
        public void ItContainsTheId()
        {
            string result = ExecuteResolve();

            Assert.True(result.IndexOf(sampleObject.Id, StringComparison.OrdinalIgnoreCase) > 0);
        }

        private string ExecuteResolve()
        {
            StreamTypeNamer sut = GetSut();

            string result = sut.Resolve(sampleObject);
            return result;
        }
    }
}