using System.Threading.Tasks;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;

namespace Tests.Fakes
{
    public class FakeObject : DomainObjectBase
    {
        public FakeObject(string id) : base(id)
        {
        }

        [Create]
        public void Foo(CreateCommand cmd)
        {
        }

        [Create]
        public Task FooTask()
        {
            return Task.FromResult(true);
        }

        [Update]
        public Task FooTaskUpdate(SampleCommand command)
        {
            return Task.FromResult(true);
        }

        public void FooNoAnnotation()
        {
        }
    }
}