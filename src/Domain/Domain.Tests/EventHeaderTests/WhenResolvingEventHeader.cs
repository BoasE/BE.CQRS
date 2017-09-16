using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Tests.EventHeaderTests
{
    public sealed class WhenResolvingEventHeader : GivenEventHeader
    {
        public WhenResolvingEventHeader()
        {
            EventHeader sut = GetSut();
        }
    }
}