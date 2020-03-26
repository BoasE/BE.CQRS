using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreSample.Controllers.Models;
using AspNetCoreSample.Domain;
using AspNetCoreSample.Domain.Commands;
using AspNetCoreSample.Domain.States;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Commands;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Controllers
{
    [Route("/api/customers")]
    public class CustomersController : Controller
    {
        private readonly ICommandBus bus;
        private readonly IDomainObjectRepository repository;

        public CustomersController(ICommandBus bus, IDomainObjectRepository repository)
        {
            this.bus = bus;
            this.repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCustomerModel model)
        {
            var customerId = Guid.NewGuid().ToString();
            
            //The bus will call the customer domain object 
            await bus.EnqueueAsync(new CreateCustomerFromApiCommand()
            {
                DomainObjectId = customerId, //Mandatory Id
                Name = model.Name
            });

            var acceptedResult = new AcceptedResult
            {
                //Give The Client a hint where to get the details( see below).
                Location = new Uri($"/api/customers/{customerId}", UriKind.Relative).ToString()
            };
            
            return acceptedResult;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var customer = await repository.Get<Customer>(id);
            var model = new ReadCustomerModel()
            {
                Name = customer.State<NameState>().Name
            };

            return new ObjectResult(model);
        }
    }
}