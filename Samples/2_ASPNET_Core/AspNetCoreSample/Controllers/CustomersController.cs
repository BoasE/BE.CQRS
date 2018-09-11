using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AspNetCoreSample.Domain.Commands;
using BE.CQRS.Domain.Commands;
using BE.FluentGuard;
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
        public async Task<IActionResult> Post([FromBody][Required] CreateCustomerModel model)
        {
            await bus.EnqueueAsync(new CreateCustomerFromApiCommand());

            return new AcceptedResult();
        }
    }

    public class CreateCustomerModel
    {
    }
}