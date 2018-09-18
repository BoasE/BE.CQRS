using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Repositories;
using MongoDB.Bson;

namespace AspNetCoreSample.Denormalizer.Repositories
{
    public sealed class CustomerRepository : MongoRepositoryBase<Customer>
    {
        private readonly IDenormalizerContext context;

        public CustomerRepository(IDenormalizerContext context) : base(context.Db, "TraineeStatistics")
        {
            this.context = context;
        }

        public Task AddCustomer(string customerId)
        {
            var dto = new Customer
            {
                Id = new ObjectId(customerId)
            };

            return Collection.InsertOneAsync(dto);
        }
    }
}