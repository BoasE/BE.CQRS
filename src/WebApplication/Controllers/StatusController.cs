using System;
using System.Threading.Tasks;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Commands;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Domain;

namespace WebApplication.Controllers
{
    [Route("status")]
    public class StatusController : Controller
    {
        private ICommandBus bus;

        public StatusController(ICommandBus bus)
        {
            this.bus = bus;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            await bus.EnqueueAsync(new CreateCommand(Guid.NewGuid().ToString()) {Value = "aa"});
            return Ok();
        }
    }
}