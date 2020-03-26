using System;
using BE.CQRS.Domain.Conventions;
using WebApplication.Domain;

namespace WebApplication
{
    [Denormalizer]
    public sealed class SampleDenormalizer
    {
        public void On(SampleCreatedEvent @event)
        {
            Console.WriteLine($"{nameof(SampleCreatedEvent)}-Event Received");
        }
    }
}