using System;
using System.Collections.Generic;
using System.Text;
using BE.CQRS.Domain.Policies;
using Domain.Events;

namespace Domain.States
{
    public class HasNoAddressPolicy : PolicyBase
    {
        public bool HasAddress { get; set; }

        public void On(AddressSetForCustomerEvent @event)
        {
            HasAddress = true;

        }

        public override bool IsValid()
        {
            return !HasAddress;
        }
    }
}
