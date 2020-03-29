using System;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.States;
using FakeItEasy;
using Tests.Fakes;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectRepositoryBaseTests
{
    public class WhenCreatingNewDomainObjects : GivenDomainObjectRepository
    {
        private IDomainObjectActivator activator = A.Fake<IDomainObjectActivator>();
        private IStateActivator stateActivator = new ActivatorDomainObjectActivator();
        private readonly Type domainObjectType;
        private const string id = "123";

        private readonly IDomainObject result;
        private readonly IDomainObject expectedResult = new SampleDomainObject(id);

        public WhenCreatingNewDomainObjects()
        {
            domainObjectType = typeof(SampleDomainObject);

            CreateFakeActivator();

            DomainObjectRepositoryBase sut = GetSut(activator,stateActivator);

            result = sut.New(domainObjectType, id);
        }

        [Fact]
        public void ItCallesTheActivator()
        {
            A.CallTo(() => activator.Resolve(A<Type>.That.IsEqualTo(domainObjectType), A<string>.That.IsEqualTo(id)))
                .MustHaveHappened();
        }

        [Fact]
        public void ItCreatesTheObject()
        {
            Assert.NotNull(result);
        }

        [Fact]
        public void ItPassedTheId()
        {
            Assert.Equal(id, result.Id);
        }

        private void CreateFakeActivator()
        {
            A.CallTo(() => activator.Resolve(A<Type>.That.IsEqualTo(domainObjectType), A<string>.That.IsEqualTo(id))).Returns(expectedResult);
        }
    }
}