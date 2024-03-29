﻿using System;

namespace BE.CQRS.Domain.Conventions
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DenormalizerAttribute : Attribute
    {
        public bool IsBackground { get; init; } = false;
    }
}