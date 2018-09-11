using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AspNetCoreSample.Domain.Commands;
using BE.CQRS.Domain.Commands;
using BE.FluentGuard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

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
            
            await bus.EnqueueAsync(new CreateCustomerFromApiCommand()
            {
                DomainObjectId = Guid.NewGuid().ToString(), //Mandatory Id
                Name = "Contoso" // Should be applied from model
            });

            return new AcceptedResult();
        }
    }

    public class CreateCustomerModel
    {
    }
}