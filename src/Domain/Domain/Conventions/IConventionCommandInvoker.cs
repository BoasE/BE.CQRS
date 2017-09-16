using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.Conventions
{
    public interface IConventionCommandInvoker
    {
        Task InvokeAndSaveAsync(Type domainObjectType, ICommand cmd,
            IEnumerable<CommandMethodMapping> commands);
    }
}