using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Data.MongoDb.Tests.StreamNamerTests
{
    public sealed class SampleBo : DomainObjectBase
    {
        public SampleBo(string id) : base(id)
        {
        }
    }
}