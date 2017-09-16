using System;
using BE.CQRS.Domain.Conventions;

namespace Testrunner
{
    [Denormalizer]
    public sealed class TestDenormalizer
    {
        public void On(MyEvent @event)
        {
            Console.WriteLine("new myevent");
        }

        public void On(SecondEvent @event)
        {
            Console.WriteLine("new secondevent");
        }
    }
}