using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using NetCoreConsoleSample.Domain;
using NetCoreConsoleSample.Domain.Commands;
using NetCoreConsoleSample.Domain.States;

namespace NetCoreConsoleSample
{
    class Program
    {
        static Random Randomizer = new Random((int)DateTime.Now.TimeOfDay.TotalMilliseconds);
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting application...");
            Console.WriteLine("Adding CQRS...");
            string url = "mongodb://localhost:27017/";

            Console.WriteLine(url);
            IMongoDatabase mongodb = new MongoClient(url).GetDatabase("hellobecqrs");
            var repo = CQRSBooter.AddCqrs(mongodb);
            var id = Randomizer.Next();
            var customer = new Customer(id.ToString());
            customer.CreateNewCustomer(new CreateCustomerFromConsoleCommand()
            {
                Name = "Contoso"
            });
            await repo.SaveAsync(customer);
            var persistedCustomer = await repo.Get<Customer>(id.ToString());
            Console.WriteLine($"Persisted customer name: {persistedCustomer.State<NameState>().Name}");
        }
    }
}
