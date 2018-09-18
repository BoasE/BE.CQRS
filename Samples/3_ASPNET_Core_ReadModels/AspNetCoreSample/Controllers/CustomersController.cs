using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreSample.Controllers.Models;
using AspNetCoreSample.Denormalizer.Repositories;
using AspNetCoreSample.Domain;
using AspNetCoreSample.Domain.Commands;
using AspNetCoreSample.Domain.States;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Commands;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AspNetCoreSample.Controllers
{
    [Route("/api/customers")]
    public class CustomersController : Controller
    {
        private readonly ICommandBus bus;
        private readonly IDomainObjectRepository repository;
        private readonly IMongoDatabase database;

        public CustomersController(ICommandBus bus, IDomainObjectRepository repository, IMongoDatabase database)
        {
            this.bus = bus;
            this.repository = repository;
            this.database = database;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCustomerModel model)
        {
            var customerId = Guid.NewGuid().ToString();
            await bus.EnqueueAsync(new CreateCustomerFromApiCommand()
            {
                DomainObjectId = customerId, //Mandatory Id
                Name = model.Name
            });

            var acceptedResult = new AcceptedResult();
            acceptedResult.Location = new Uri($"/api/customers/{customerId}", UriKind.Relative).ToString();

            return acceptedResult;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var customer = await database.GetCollection<CustomerReadModel>("Customers")
                .Find(model => model.CustomerId == id).SingleAsync();

            return new ObjectResult(customer);
        }
    }
}