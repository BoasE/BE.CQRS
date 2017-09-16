using System.Collections.Generic;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Tests.StateTests
{
    public class GivenState
    {
        protected SampleState GetSut()
        {
            return new SampleState();
        }

        protected SampleState ResolveState(IEnumerable<IEvent> events)
        {
            SampleState sut = GetSut();

            sut.Execute(events);

            return sut;
        }
    }
}