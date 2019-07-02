﻿using System;
using AspNetCoreSample.Domain.Commands;
using AspNetCoreSample.Domain.Events;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.DomainObjects;

namespace AspNetCoreSample.Domain
{
    public sealed class Customer : DomainObjectBase
    {
        public Customer(string id) : base(id, null)
        {
        }

        [Create]
        public void CreateNewCustomer(CreateCustomerFromApiCommand cmd)
        {
            if(string.IsNullOrWhiteSpace(cmd.Name))
            {
                throw new InvalidOperationException("A customer can only created with a valid name!");   
            }

            Console.WriteLine("Creating Created event.");
            RaiseEvent<CustomerCreatedFromApiEvent>(x =>
            {
                x.Name = cmd.Name;
            });
        }
    }
}
