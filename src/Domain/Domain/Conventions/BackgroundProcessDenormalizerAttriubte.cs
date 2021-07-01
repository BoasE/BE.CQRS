using System;

namespace BE.CQRS.Domain.Conventions
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BackgroundProcessDenormalizerAttribute : Attribute
    {
    }
}