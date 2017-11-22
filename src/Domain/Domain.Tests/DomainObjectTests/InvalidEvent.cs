using System;
using System.Collections.Generic;
using System.Text;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class InvalidEvent : EventBase
    {
        public override bool Validate() => false;
    }
}
