using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Repositories;
using MongoDB.Bson;

namespace AspNetCoreSample.Denormalizer.Repositories
{
    public sealed class CustomerRepository : MongoRepositoryBase<Customer>
    {
        private readonly IDenormalizerContext context;

        public CustomerRepository(IDenormalizerContext context) : base(context.Db, "Customer")
        {
            this.context = context;
        }

        public Task AddCustomer(string customerId, string name)
        {
            var dto = new Customer
            {
                CustomerId = customerId,
                Name = name
            };

            return Collection.InsertOneAsync(dto);
        }
    }
}