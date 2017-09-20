using System;

namespace BE.CQRS.Domain.Policies
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequiresAttribute : Attribute
    {
        public Type[] Polices { get; set; }

        public RequiresAttribute()
        {
        }

        public RequiresAttribute(params Type[] policy)
        {
            Polices = policy;
        }
    }
}