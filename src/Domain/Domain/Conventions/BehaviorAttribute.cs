using System;

namespace BE.CQRS.Domain.Conventions
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class BehaviorAttribute : Attribute
    {
    }
}