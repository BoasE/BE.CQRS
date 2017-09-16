using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;

namespace Testrunner
{
    public sealed class SampleBo : DomainObjectBase
    {
        public SampleBo(string id) : base(id)
        {
        }

        [Create]
        public void Execute()
        {
            RaiseEvent<MyEvent>(x => x.Id = "1231231");
        }

        [Update]
        public void Next()
        {
            RaiseEvent<SecondEvent>(x =>
            {
            });
        }
    }
}