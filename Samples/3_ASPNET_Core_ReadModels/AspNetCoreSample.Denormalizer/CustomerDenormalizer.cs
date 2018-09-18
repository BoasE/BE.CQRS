using System;
using System.Threading.Tasks;
using AspNetCoreSample.Denormalizer.Repositories;
using AspNetCoreSample.Domain.Events;
using BE.CQRS.Domain.Conventions;
using BE.CQRS.Domain.Events;

namespace AspNetCoreSample.Denormalizer
{
    [Denormalizer]
    public sealed class CustomerDenormalizer
    {
        private readonly IDenormalizerContext context;

        private readonly CustomerRepository repository;

        public CustomerDenormalizer(IDenormalizerContext context)
        {
            this.context = context;
            this.repository = new CustomerRepository(context);
        }

        public async Task On(CustomerCreatedFromApiEvent @event)
        {
            var id = @event.Headers.GetString(EventHeaderKeys.AggregateId);
            await repository.AddCustomer(id);

            Console.WriteLine("started");
        }
    }
}