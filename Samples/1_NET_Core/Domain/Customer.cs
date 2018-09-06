using System;
using BE.CQRS.Domain.DomainObjects;
using Domain.Commands;
using Domain.Events;

namespace Domain
{
    public sealed class Customer : DomainObjectBase
    {
        public Customer(string id) : base(id, null)
        {
        }

        public void CreateNewCustomer(CreateCustomerFromConsoleCommand cmd)
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
    }
}
