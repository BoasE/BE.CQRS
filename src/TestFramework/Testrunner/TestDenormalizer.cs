using System;
using BE.CQRS.Domain.Conventions;

namespace Testrunner
{
    [Denormalizer]
    public sealed class TestDenormalizer
    {
        public void On(MyEvent @event)
        {
            
        }

        public void On(SecondEvent @event)
        {
            
        }
    }
}