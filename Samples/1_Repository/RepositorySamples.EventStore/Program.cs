using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BE.CQRS.Domain;
using Domain.Commands;
using RepositorySamples.EventStore.Domain;

namespace RepositorySamples.EventStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating Repository...");
            var repo = Bootstrap.CreateDomainObjectRepository();

            Console.WriteLine("Repository created, running customer workflow...");
            Task.Run(() => CreateCustomer(repo));
            
            Console.WriteLine("Key to exit...");
            Console.ReadKey(true);
        }

        static async Task CreateCustomer(IDomainObjectRepository repo)
        {
            string customerId = Guid.NewGuid().ToString();

            await CreateCustomer(repo, customerId); // Create a Customer

            var domainObject = await repo.Get<Customer>(customerId); //now read it from the db

            await SetAddress(repo, domainObject); // Update the address
            await SetAddress(repo, domainObject); // This one will not work due to the policy in the customer domainobject method
        }

        private static async Task SetAddress(IDomainObjectRepository repo, Customer domainObject)
        {
            var cmd = new AddAddressCommand() {City = "Karlsruhe", Street = "Foo"};
            domainObject.SetAddress(cmd);
            await repo.SaveAsync(domainObject);
        }

        private static async Task CreateCustomer(IDomainObjectRepository repo, string customerId)
        {
            var createCommand = new CreatCustomerFromConsoleCommand()
            {
                Name = "MyCustomer"
            };

            var
                customer = new Customer(
                    customerId); // Later you would instanciate a customer on your own. This will be done in further examples by CommandBus and CommandPipelines automatically.

            Console.WriteLine("Executing DomainObject...");
            customer.CreateNewCustomer(createCommand);

            Console.WriteLine("Saving Customer...");

            string url =
                $"http://localhost:2113/web/index.html#/streams/{Bootstrap.Prefix}-{customer.GetType().Name}-{customer.Id}";

            Console.WriteLine($"Visit {url} to see the created stream"); //Todo add correct url
            await repo.SaveAsync(customer);
        }
    }
}
