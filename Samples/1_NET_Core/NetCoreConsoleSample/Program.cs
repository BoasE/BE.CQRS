using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NetCoreConsoleSample.Domain;
using NetCoreConsoleSample.Domain.Commands;
using NetCoreConsoleSample.Domain.States;

namespace NetCoreConsoleSample
{
    class Program
    {
        static Random Randomizer = new Random((int) DateTime.Now.TimeOfDay.TotalMilliseconds);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting application...");
            Console.WriteLine("Adding CQRS...");

            var services = ConfgServices();
            var serviceProvider = services.BuildServiceProvider();

            var repo = serviceProvider.GetRequiredService<IDomainObjectRepository>();

            var customer = CreateCustomerObject();
            await repo.SaveAsync(customer);
            var persistedCustomer = await repo.Get<Customer>(customer.Id);

            Console.WriteLine($"Persisted customer name: {persistedCustomer.State<NameState>().Name}");
        }

        private static IServiceCollection ConfgServices()
        {
            var factory = LoggerFactory.Create(x => x.AddConsole());

            var services = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(factory)
                .AddEventSource()
                .AddDenormalizer();

            return services;
        }

        private static Customer CreateCustomerObject()
        {
            Customer customer;

            var id = Randomizer.Next();
            customer = new Customer(id.ToString());
            customer.CreateNewCustomer(new CreateCustomerFromConsoleCommand()
            {
                Name = "Contoso"
            });
            return customer;
        }
    }
}