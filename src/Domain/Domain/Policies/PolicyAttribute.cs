using System;

namespace BE.CQRS.Domain.Policies
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PolicyAttribute : Attribute
    {
        public Type[] Polices { get; set; }

        public PolicyAttribute()
        {
        }

        public PolicyAttribute(params Type[] policy)
        {
            Polices = policy;
        }
    }
}