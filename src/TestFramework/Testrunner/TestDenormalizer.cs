using System;
using BE.CQRS.Domain.Conventions;

namespace Testrunner
{
    [Denormalizer]
    public sealed class TestDenormalizer
    {
        public void On(MyEvent @event)
        {
            throw new InvalidOperationException();
            Console.WriteLine("new myevent");
        }

        public void On(SecondEvent @event)
        {
            throw new InvalidOperationException();
            Console.WriteLine("new secondevent");
        }
    }
}