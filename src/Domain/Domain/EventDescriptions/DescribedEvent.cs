using System;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.EventDescriptions
{
    public sealed record DescribedEvent(string Title, string Message, DateTimeOffset Timestamp,IEvent Event,bool WasDescribed);
}