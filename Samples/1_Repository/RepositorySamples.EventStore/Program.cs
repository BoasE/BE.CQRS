using System;
using System.Threading.Tasks;
using BE.CQRS.Domain;
using RepositorySamples.EventStore.Domain;

namespace RepositorySamples.EventStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating Repository...");
            var repo = Bootstrap.CreateDomainObjectRepository();

            Console.WriteLine("Repository created, running customer creation...");
            Task.Run(() => CreateCustomer(repo));

            Console.WriteLine("Customer Saved!");
          
            Console.WriteLine("Key to exit...");
            Console.ReadKey(true);
        }

        static Task CreateCustomer(IDomainObjectRepository repo)
        {
            var createCommand = new CreatCustomerFromConsoleCommand()
            {
                Name = "MyCustomer"
            };

            var customer = new Customer(Guid.NewGuid().ToString()); // Later you would instanciate a customer on your own. This will be done in further examples by CommandBus and CommandPipelines automatically.

            Console.WriteLine("Executing DomainObject...");
            customer.CreateNewCustomer(createCommand);

            Console.WriteLine("Saving Customer...");

            string url =
                $"http://localhost:2113/web/index.html#/streams/{Bootstrap.Prefix}-{customer.GetType().Name}-{customer.Id}";

            Console.WriteLine($"Visit {url} to see the created stream"); //Todo add correct url
            return repo.SaveAsync(customer);
          
        }
    }
}
