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

        public CustomerDenormalizer(IDenormalizerContext context)
        {
            this.context = context;
        }

        public Task On(CustomerCreatedFromApiEvent @event)
        {
            var id = @event.Headers.GetString(EventHeaderKeys.AggregateId);
            var customer = new CustomerReadModel {CustomerId = id, Name = @event.Name};
            return context.Db.GetCollection<CustomerReadModel>("Customers").InsertOneAsync(customer);
        }
    }
}