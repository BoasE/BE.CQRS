using System;
using System.Reflection;
using BE.CQRS.Domain.Events.Handlers;

namespace BE.CQRS.Domain.Tests.ConventionEventhandlerTests
{
    public class GivenConventionEventHandler
    {
        public ConventionEventHandler GetSut(params Assembly[] asms)
        {
            return new ConventionEventHandler(Activator.CreateInstance, asms);
        }
    }
}