using System;
using System.Threading.Tasks;
using AspNetCoreSample.Controllers.Models;
using AspNetCoreSample.Domain.Commands;
using BE.CQRS.Domain.Commands;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Controllers
{
    [Route("/api/customers")]
    public class CustomersController : Controller
    {
        private readonly ICommandBus bus;

        public CustomersController(ICommandBus bus)
        {
            this.bus = bus;
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
    }
}