using System;
using System.Collections.Generic;
using System.Text;
using BE.CQRS.Domain.Events;

namespace Domain.Events
{
    public sealed class AddressSetForCustomerEvent  : EventBase
    {

        public string City { get; set; }

        public string Street { get; set; }
    }
}
