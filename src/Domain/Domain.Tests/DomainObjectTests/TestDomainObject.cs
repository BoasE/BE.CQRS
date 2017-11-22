using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public sealed class TestDomainObject : DomainObjectBase

    {
        public TestDomainObject(string id) : base(id)
        {
        }

        [Update]
        public void RaiseEvent()
        {
            RaiseEvent<TestEvent>(i =>
            {
            });
        }


        [Update]
        public void RaiseInvalidEvent()
        {
            RaiseEvent<InvalidEvent>(i =>
            {
            });
        }
    }
}