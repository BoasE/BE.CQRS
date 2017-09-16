using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;

namespace Tests.Fakes
{
    public class SampleDomainObject : DomainObjectBase
    {
        public SampleDomainObject(string id) : base(id)
        {
        }

        [Create]
        public void Foo(CreateCommandSecond cmd)
        {
            RaiseEvent<SampleCreateEvent>(e =>
            {
            });
        }
    }
}