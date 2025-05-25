using System;
using BE.CQRS.Domain.Commands;

namespace Tests.Fakes
{
    public class CreateCommandSecond : ICommand
    {
        public string DomainObjectId { get; set; } = Guid.CreateVersion7().ToString();
    }
}