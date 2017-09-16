using System;

namespace BE.CQRS.Domain.Conventions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UpdateAttribute : BehaviorAttribute
    {
    }
}