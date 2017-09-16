using System;
using System.Collections.Generic;
using System.Text;
using BE.CQRS.Domain;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Policies;
using Domain.Commands;
using Domain.Events;
using Domain.States;
using RepositorySamples.EventStore.Domain;

namespace RepositorySamples.EventStore
{
    public sealed class Customer : DomainObjectBase
    {
        public Customer(string id, IEventMapper mapper = null) : base(id, mapper)
        {
        }

        public void CreateNewCustomer(CreatCustomerFromConsoleCommand cmd)
        {
            if(string.IsNullOrWhiteSpace(cmd.Name))
            {
                throw new InvalidOperationException("A customer can only created with a valid name!");   
            }

            Console.WriteLine("Creating Created event.");
            RaiseEvent<CustomerCreatedFromConsoleEvent>(x =>
            {
                x.Name = cmd.Name;
            });
        }

        [Requires(typeof(HasNoAddressPolicy))]
        public void SetAddress(AddAddressCommand cmd)
        {
            RaiseEvent<AddressSetForCustomerEvent>(x =>
            {
                x.City = cmd.City;
                x.Street = cmd.Street;
            });
        }
    }
}
