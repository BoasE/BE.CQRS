namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class GivenDomainObject
    {
        protected TestDomainObject GetSut(string id)
        {
            return new TestDomainObject(id);
        }
    }
}